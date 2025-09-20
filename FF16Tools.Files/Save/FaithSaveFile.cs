using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO.Hashing;

using Syroot.BinaryData.Memory;
using Syroot.BinaryData;

using Ionic.Zlib;

using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;

namespace FF16Tools.Files.Save;

public class FaithSaveGameData
{
    public const uint MAIN_HEADER_SIZE = 0x10;
    public const uint FILE_ENTRY_SIZE = 0x20;

    public const string PNG_CHUNK_FAITH = "faTh";

    private Dictionary<string, byte[]> _files = [];

    public IReadOnlyDictionary<string, byte[]> Files => _files;

    public static FaithSaveGameData Open(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!File.Exists(path))
            throw new FileNotFoundException("Save file does not exist.");

        FaithSaveGameData saveFile = new FaithSaveGameData();

        var ms = File.OpenRead(path);

        // Yep. They're storing SAVE DATA in a PNG CHUNK.
        var pngReader = new PngReader(ms);
        ChunksList chunks = pngReader.GetChunksList();
        PngChunkUNKNOWN faithChunk = (PngChunkUNKNOWN)chunks.GetById1(PNG_CHUNK_FAITH, false);
        if (faithChunk is null)
            throw new KeyNotFoundException($"Faith Chunk '{PNG_CHUNK_FAITH}' was not found in PNG Save File. Is this a valid save file?");

        byte[] saveData = faithChunk.GetData();

        saveFile.ReadSaveData(saveData);
        return saveFile;
    }

    public void AddFile(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        byte[] fileData = File.ReadAllBytes(file);
        if (fileData.Length < 0x10)
            throw new ArgumentException("File must be at least 16 bytes in size.");

        _files.Add(Path.GetFileName(file), File.ReadAllBytes(file));
    }

    private void ReadSaveData(byte[] saveData)
    {
        SpanReader sr = new SpanReader(saveData);
        uint headerSize = sr.ReadUInt32();
        sr.ReadUInt32();
        uint methodMagic = sr.ReadUInt32();
        uint numFiles = sr.ReadUInt32();

        for (uint i = 0; i < numFiles; i++)
        {
            sr.Position = (int)(MAIN_HEADER_SIZE + (i * FILE_ENTRY_SIZE));
            uint fileNameLength = sr.ReadUInt32();
            uint dataLength = sr.ReadUInt32();
            long fileNamePtr = sr.ReadInt64();
            long decompressedDataLength = sr.ReadInt64();
            long dataPtr = sr.ReadInt64();

            Crypt(saveData.AsSpan((int)fileNamePtr, (int)fileNameLength));
            sr.Position = (int)fileNamePtr;
            string fileName = sr.ReadString0();

            sr.Position = (int)dataPtr;
            byte[] inputData = sr.ReadBytes((int)dataLength);
            byte[] outputBuffer = new byte[decompressedDataLength];
            DecryptDecompress(inputData, outputBuffer);

            _files.Add(fileName, outputBuffer);
        }
    }

    public byte[] WriteSaveFile()
    {
        byte[] saveData = WriteSaveData();

        using var ms = new MemoryStream();
        var pngWriter = new PngWriter(ms, new ImageInfo(776, 436, 8, true));
        pngWriter.IdatMaxSize = 0x2000;

        var faithChunk = new PngChunkUNKNOWN(PNG_CHUNK_FAITH, pngWriter.ImgInfo);
        faithChunk.ChunkGroup = 1;
        faithChunk.SetData(saveData);
        pngWriter.GetChunksList().Queue(faithChunk);
        
        for (int i = 0; i < pngWriter.ImgInfo.Rows; i++)
        {
            pngWriter.WriteRow(new int[pngWriter.ImgInfo.Cols * 4]);
        }

        pngWriter.End();

        return ms.ToArray();
    }

    private byte[] WriteSaveData()
    {
        FixChecksums();

        uint tocSize = MAIN_HEADER_SIZE + ((uint)Files.Count * FILE_ENTRY_SIZE);
        using var ms = new MemoryStream();
        using var bs = new BinaryStream(ms, ByteConverter.Little);
        bs.WriteUInt32(tocSize); // Toc size
        bs.WriteUInt32(0);
        bs.WriteUInt32(0x46494D55); // UMIF (magic, must match)
        bs.WriteUInt32((uint)Files.Count);

        ulong lastDataOffset = MAIN_HEADER_SIZE + ((ulong)Files.Count * FILE_ENTRY_SIZE);

        int i = 0;
        foreach (KeyValuePair<string, byte[]> kv in Files)
        {
            uint strLen = (uint)Encoding.UTF8.GetByteCount(kv.Key) + 1;
            bs.Position = MAIN_HEADER_SIZE + (i * FILE_ENTRY_SIZE);
            bs.WriteUInt32(strLen);
            bs.WriteUInt32(0); // data size write later
            bs.WriteUInt64(lastDataOffset); // name offset

            bs.Position = (long)lastDataOffset;
            byte[] encryptedString = new byte[strLen];
            Encoding.UTF8.GetBytes(kv.Key, encryptedString);
            Crypt(encryptedString);
            bs.Write(encryptedString);
            bs.Align(0x04, grow: true);

            lastDataOffset = (ulong)bs.Position;
            i++;
        }

        i = 0;
        foreach (KeyValuePair<string, byte[]> kv in Files)
        {
            Span<byte> compressed = CompressEncrypt(kv.Value);
            bs.Position = MAIN_HEADER_SIZE + (i * FILE_ENTRY_SIZE) + 0x04;
            bs.WriteUInt32((uint)compressed.Length);
            bs.Position += 8;
            bs.WriteUInt64((ulong)kv.Value.Length);
            bs.WriteUInt64(lastDataOffset);

            bs.Position = (long)lastDataOffset;
            bs.Write(compressed);
            bs.WriteByte(0);

            lastDataOffset = (ulong)bs.Position;
            i++;
        }

        return ms.ToArray();
    }

    private void FixChecksums()
    {
        // Game uses this specific bit to determine whether to load as xml or root.
        // Xmls don't have checksums.
        var xmlRoot = "<?xml version=\"1.0\"?>"u8;
        foreach (var file in _files)
        {
            if (file.Value.AsSpan(0, xmlRoot.Length).SequenceEqual(xmlRoot))
                continue;

            Span<byte> toCrc = file.Value.AsSpan(0x10, file.Value.Length - 0x10);
            MemoryMarshal.Cast<byte, uint>(file.Value)[1] = Crc32.HashToUInt32(toCrc);
        }
    }

    // FF16 Demo 1.0.0: 1409DD0DC
    private static void DecryptDecompress(byte[] encryptedData, byte[] decompressedBuffer)
    {
        // This is pretty much accurate to how the game does it.
        // Allocate a buffer that fits the data + the zlib header (2 bytes).
        byte[] compressedBuffer = new byte[2 + encryptedData.Length];

        // Set the zlib header. This will set the fdict flag
        MemoryMarshal.Cast<byte, ushort>(compressedBuffer)[0] = 0xF978;

        // Decrypt and set the decrypted data to the whole buffer.
        Crypt(encryptedData);
        encryptedData.CopyTo(compressedBuffer.AsSpan(2));

        Decompress(compressedBuffer, decompressedBuffer);
    }

    private static Span<byte> CompressEncrypt(byte[] decData)
    {
        Span<byte> compressed = Compress(decData);
        Crypt(compressed);
        return compressed;
    }

    private static void Decompress(byte[] inputBuffer, byte[] outputBuffer)
    {
        var cod = new ZlibCodec(Ionic.Zlib.CompressionMode.Decompress);
        cod.InitializeInflate(15, true);
        cod.InputBuffer = inputBuffer;
        cod.OutputBuffer = outputBuffer;
        cod.AvailableBytesOut = outputBuffer.Length;
        cod.AvailableBytesIn = inputBuffer.Length;
        if (cod.Inflate(FlushType.Finish) == 2)
        {
            cod.SetDictionary(CompressDict.ZlibDict);
            cod.Inflate(FlushType.Finish);
        }
    }

    private static Span<byte> Compress(byte[] inputBuffer)
    {
        byte[] outputBuffer = new byte[inputBuffer.Length * 2];

        var cod = new ZlibCodec(Ionic.Zlib.CompressionMode.Compress);
        cod.InitializeDeflate();
        cod.SetDeflateParams(Ionic.Zlib.CompressionLevel.BestCompression, CompressionStrategy.Default); // Accurate - produces 0x78F9

        cod.InputBuffer = inputBuffer;
        cod.OutputBuffer = outputBuffer;
        cod.AvailableBytesOut = outputBuffer.Length;
        cod.AvailableBytesIn = inputBuffer.Length;
        cod.SetDictionary(CompressDict.ZlibDict);
        cod.Deflate(FlushType.Finish);

        return outputBuffer.AsSpan(2, (int)cod.TotalBytesOut); // Skip 2 byte magic
    }

    public const ulong XOR_KEY = 0xF3F80FE5F1FC4F3;
    public static void Crypt(Span<byte> data)
    {
        Span<byte> cur = data;
        while (cur.Length >= 8)
        {
            MemoryMarshal.Cast<byte, ulong>(cur)[0] ^= XOR_KEY;
            cur = cur[8..];
        }

        if (cur.Length >= 4)
        {
            MemoryMarshal.Cast<byte, uint>(cur)[0] ^= (uint)(XOR_KEY & 0xFFFFFFFF);
            cur = cur[4..];
        }

        if (cur.Length >= 2)
        {
            MemoryMarshal.Cast<byte, ushort>(cur)[0] ^= (ushort)(XOR_KEY & 0xFFFF);
            cur = cur[2..];
        }

        if (cur.Length >= 1)
            cur[0] ^= (byte)(XOR_KEY & 0xFF);
    }
}
