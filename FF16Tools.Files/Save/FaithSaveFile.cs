using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Syroot.BinaryData.Memory;

using Ionic.Zlib;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;

namespace FF16Tools.Files.Save;

public class FaithSaveGameData
{
    public const string PNG_CHUNK_FAITH = "faTh";

    private Dictionary<string, byte[]> _files { get; set; } = [];
    public IReadOnlyDictionary<string, byte[]> Files => _files;

    public static FaithSaveGameData Open(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!File.Exists(path))
            throw new FileNotFoundException("Save file does not exist.");

        FaithSaveGameData saveFile = new FaithSaveGameData();

        using var ms = File.OpenRead(path);

        // Yep. They're storing SAVE DATA in a PNG CHUNK.
        var pngReader = new PngReader(ms);
        ChunksList chunks = pngReader.GetChunksList();
        PngChunkUNKNOWN faithChunk = (PngChunkUNKNOWN)chunks.GetById1(PNG_CHUNK_FAITH, false);
        byte[] saveData = faithChunk.GetData();

        SpanReader sr = new SpanReader(saveData);
        uint headerSize = sr.ReadUInt32();
        sr.ReadUInt32();
        uint methodMagic = sr.ReadUInt32();
        uint numFiles = sr.ReadUInt32();

        for (int i = 0; i < numFiles; i++)
        {
            sr.Position = 0x10 + (i * 0x20);
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

            saveFile._files.Add(fileName, outputBuffer);
        }

        return saveFile;
    }

    // FF16 Demo 1.0.0: 1409DD0DC
    public static void DecryptDecompress(byte[] encryptedData, byte[] decompressedBuffer)
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
