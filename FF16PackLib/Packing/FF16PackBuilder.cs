using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Buffers;

using Vortice.DirectStorage;

using SharpGen.Runtime;
using FF16PackLib.Hashing;
using FF16PackLib.Crypto;

using Syroot.BinaryData.Memory;
using Syroot.BinaryData;

namespace FF16PackLib.Packing;

public class FF16PackBuilder
{
    public List<FileTask> _packFileTasks { get; set; } = [];
    public List<ChunkTask> _sharedChunksTasks { get; set; } = [];
    private ChunkTask _lastSharedChunk;
    private byte[] _stringTable;
    private long _lastMultiChunkHeaderOffset;

    private PackBuildOptions _options;

    private readonly Dictionary<FF16PackDStorageChunk, ulong> _sharedChunksToOffsets = [];

    private static IDStorageCompressionCodec _codec;
    static FF16PackBuilder()
    {
        _codec = DirectStorage.DStorageCreateCompressionCodec(CompressionFormat.GDeflate, (uint)Environment.ProcessorCount);
    }

    public FF16PackBuilder(PackBuildOptions options = null)
    {
        _options = options ?? new();
    }

    public void InitFromDirectory(string dir)
    {
        var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Order().ToList();
        foreach (var file in files)
        {
            string gamePath = file[(dir.Length + 1)..].ToLower().Replace('\\', '/');
            Console.WriteLine($"PACK: Adding '{gamePath}'...");

            var fileInfo = new FileInfo(file);
            var task = new FileTask()
            {
                LocalPath = file,
                GamePath = gamePath,
            };
            task.PackFile.DecompressedFileSize = (ulong)fileInfo.Length;

            _packFileTasks.Add(task);
        }

        BuildSharedChunks();
        BuildStringTable();
    }

    private void BuildSharedChunks()
    {
        int i = 0;
        foreach (var file in _packFileTasks)
        {
            if (!IsCompressionForFileSuggested(file.GamePath))
                continue;

            if (file.PackFile.DecompressedFileSize >= FF16Pack.MIN_FILE_SIZE_FOR_MULTIPLE_CHUNKS)
            {
                long numSplitChunks = (long)file.PackFile.DecompressedFileSize / FF16Pack.MAX_DECOMPRESSED_MULTI_CHUNK_SIZE;
                if (file.PackFile.DecompressedFileSize % FF16Pack.MAX_DECOMPRESSED_MULTI_CHUNK_SIZE > 0)
                    numSplitChunks++;

                file.SplitChunkOffsets = new uint[numSplitChunks];
            }
            else if (file.PackFile.DecompressedFileSize < FF16Pack.MAX_FILE_SIZE_FOR_SHARED_CHUNK)
            {
                Console.WriteLine($"PACK: Compressing '{file.GamePath}' into shared chunk..");
                _lastSharedChunk ??= new ChunkTask();

                if (_lastSharedChunk.PackChunk.DecompressedSize + (long)file.PackFile.DecompressedFileSize > FF16Pack.MAX_DECOMPRESSED_SHARED_CHUNK_SIZE)
                {
                    _sharedChunksTasks.Add(_lastSharedChunk);
                    _lastSharedChunk = new ChunkTask();
                }

                file.PackFile.DataOffset = _lastSharedChunk.PackChunk.DecompressedSize;
                file.SharedChunk = _lastSharedChunk;
                _lastSharedChunk.PackChunk.DecompressedSize += (uint)file.PackFile.DecompressedFileSize;
                _lastSharedChunk.Files.Add(file);

            }

            i++;
        }

        if (_lastSharedChunk is not null && _lastSharedChunk.PackChunk.DecompressedSize != 0)
            _sharedChunksTasks.Add(_lastSharedChunk);
    }

    private void BuildStringTable()
    {
        using var ms = new MemoryStream();
        using var bs = new BinaryStream(ms);
        foreach (var file in _packFileTasks)
        {
            file.StringTableNameOffsetRelative = bs.Position;
            bs.WriteString(file.GamePath, StringCoding.ZeroTerminated);
        }

        _stringTable = ms.ToArray();
    }

    private uint CalculateHeaderSize()
    {
        uint size = FF16Pack.HEADER_SIZE + (uint)(_packFileTasks.Count * FF16PackFile.GetSize()) +
            CalculateSizeOfMultiChunkHeaders() +
            (uint)(_sharedChunksTasks.Count * FF16PackDStorageChunk.GetSize()) +
            (uint)_stringTable.Length;

        return size;
    }

    private uint CalculateSizeOfMultiChunkHeaders()
    {
        uint size = 0;
        for (int i = 0; i < _packFileTasks.Count; i++)
        {
            if (_packFileTasks[i].SplitChunkOffsets is null)
                continue;

            size += (uint)(8u + (_packFileTasks[i].SplitChunkOffsets.Length * sizeof(uint)));
        }

        return size;
    }

    public void WriteTo(string file)
    {
        Console.WriteLine("PACK: Starting write.");

        using var fs = new FileStream(file, FileMode.Create);
        uint headerLength = CalculateHeaderSize();

        _lastMultiChunkHeaderOffset = FF16Pack.HEADER_SIZE + (uint)(_packFileTasks.Count * FF16PackFile.GetSize());

        // Start writing the files first
        fs.Position = (long)headerLength;

        for (int i = 0; i < _sharedChunksTasks.Count; i++)
        {
            Console.WriteLine($"PACK: Writing shared chunk {i+1}/{_sharedChunksTasks.Count}..");
            ChunkTask? chunk = _sharedChunksTasks[i];
            WriteSharedChunk(fs, chunk);
        }

        foreach (FileTask task in _packFileTasks)
        {
            if (task.SharedChunk is not null)
                continue; // Already written

            WriteFile(fs, task);
        }

        byte[] header = new byte[(int)headerLength];
        WriteHeader(fs, header);
    }

    private void WriteHeader(FileStream packStream, byte[] headerBuffer)
    {
        Console.WriteLine("PACK: Writing header.");

        if (_options.Encrypt)
            Console.WriteLine("PACK: Header encryption is enabled.");
        if (!string.IsNullOrEmpty(_options.Name))
            Console.WriteLine($"PACK: Setting internal pack name to '{_options.Name}'.");

        var ms = new MemoryStream(headerBuffer);
        var bs = new BinaryStream(ms);
        bs.WriteUInt32(FF16Pack.MAGIC);
        bs.WriteUInt32((uint)headerBuffer.Length);
        bs.WriteUInt32((uint)_packFileTasks.Count);
        bs.WriteBoolean(_sharedChunksTasks.Count > 0);
        bs.WriteBoolean(_options.Encrypt);
        bs.WriteUInt16((ushort)_sharedChunksTasks.Count);
        bs.WriteUInt64(0); // Pack size, write later
        
        byte[] nameBuffer = new byte[0x100];
        Encoding.UTF8.GetBytes(_options.Name, nameBuffer);
        if (_options.Encrypt)
            XorEncrypt.CryptHeaderPart(nameBuffer);
        bs.WriteBytes(nameBuffer);

        // Write the string table first
        long stringTableOffset = FF16Pack.HEADER_SIZE + (_packFileTasks.Count * FF16PackFile.GetSize()) + (_sharedChunksTasks.Count * FF16PackDStorageChunk.GetSize()) + CalculateSizeOfMultiChunkHeaders();
        bs.Position = stringTableOffset;

        for (int i = 0; i < _packFileTasks.Count; i++)
        {
            FileTask fileTask = _packFileTasks[i];
            fileTask.PackFile.FileNameOffset = (ulong)bs.Position;
            fileTask.PackFile.FileNameHash = Fnv1Hash.HashPath(fileTask.GamePath);
            bs.WriteString(fileTask.GamePath, StringCoding.ZeroTerminated);
        }
        long stringTableSize = bs.Position - stringTableOffset;

        if (_options.Encrypt)
            XorEncrypt.CryptHeaderPart(headerBuffer.AsSpan((int)stringTableOffset, (int)stringTableSize));

        // Then, write the shared chunks
        long chunkTableOffset = FF16Pack.HEADER_SIZE + (_packFileTasks.Count * FF16PackFile.GetSize()) + CalculateSizeOfMultiChunkHeaders();
        bs.Position = chunkTableOffset;
        for (int i = 0; i < _sharedChunksTasks.Count; i++)
        {
            var chunkTask = _sharedChunksTasks[i];
            _sharedChunksToOffsets.Add(chunkTask.PackChunk, (ulong)bs.Position);
            chunkTask.PackChunk.NumFilesInChunk = (ushort)chunkTask.Files.Count;
            chunkTask.PackChunk.ChunkIndex = (ushort)i;
            chunkTask.PackChunk.Write(bs);
        }

        // Then, multi chunks
        bs.Position = FF16Pack.HEADER_SIZE + (_packFileTasks.Count * FF16PackFile.GetSize());
        for (int i = 0; i < _packFileTasks.Count; i++)
        {
            if (_packFileTasks[i].SplitChunkOffsets is null)
                continue;

            bs.WriteUInt32((uint)_packFileTasks[i].SplitChunkOffsets.Length);
            bs.WriteUInt32((3 << 24) | _packFileTasks[i].SplitLastChunkSize); // TODO: figure out what this '3' is.

            for (int j = 0; j < _packFileTasks[i].SplitChunkOffsets.Length; j++)
            {
                bs.WriteUInt32(_packFileTasks[i].SplitChunkOffsets[j]);
            }
        }

        // The file infos of course
        bs.Position = FF16Pack.HEADER_SIZE;
        for (int i = 0; i < _packFileTasks.Count; i++)
        {
            FileTask fileTask = _packFileTasks[i];
            if (fileTask.SharedChunk is not null)
                fileTask.PackFile.ChunkDefOffset = _sharedChunksToOffsets[fileTask.SharedChunk.PackChunk];

            fileTask.PackFile.Write(bs);
        }

        // Finalize offsets
        bs.Position = 0x118;
        bs.WriteInt64(_sharedChunksTasks.Count > 0 ? chunkTableOffset : 0);
        bs.WriteInt64(stringTableOffset);
        bs.WriteInt64(stringTableSize);

        // Pak size
        bs.Position = 0x10;
        bs.WriteInt64(packStream.Length);

        packStream.Position = 0;
        packStream.Write(headerBuffer);
    }

    public void WriteFile(FileStream packStream, FileTask task)
    {
        if (!IsCompressionForFileSuggested(task.GamePath))
        {
            Console.WriteLine($"PACK: Writing raw '{task.GamePath}'..");

            task.PackFile.DataOffset = (ulong)packStream.Position;

            using var fileStream = File.Open(task.LocalPath, FileMode.Open);
            uint crc = CopyToWithChecksum(fileStream, packStream);
            task.PackFile.CRC32Checksum = crc;
            task.PackFile.CompressedFileSize = (uint)task.PackFile.DecompressedFileSize;
        }
        else if ((long)task.PackFile.DecompressedFileSize < FF16Pack.MIN_FILE_SIZE_FOR_MULTIPLE_CHUNKS)
        {
            Console.WriteLine($"PACK: Compressing '{task.GamePath}' into unique chunk..");

            byte[] fileBytes = File.ReadAllBytes(task.LocalPath);

            task.PackFile.DataOffset = (ulong)packStream.Position;
            task.PackFile.CRC32Checksum = Crc32.HashToUInt32(fileBytes);

            long sizeCompressed = _codec.CompressBufferBound((long)task.PackFile.DecompressedFileSize * 2); // Incase
            byte[] compBuffer = ArrayPool<byte>.Shared.Rent((int)sizeCompressed); // Incase


            unsafe
            {
                fixed (byte* inputDecompChunkPtr = fileBytes)
                fixed (byte* outputCompChunkPtr = compBuffer)
                {
                    CompressBuffer(_codec, (nint)inputDecompChunkPtr, (long)task.PackFile.DecompressedFileSize,
                        Compression.BestRatio, // Matches original
                        (nint)outputCompChunkPtr, sizeCompressed, out long compressedDataSize);

                    packStream.Write(compBuffer, 0, (int)compressedDataSize);
                    task.PackFile.CompressedFileSize = (uint)compressedDataSize;
                    task.PackFile.ChunkedCompressionFlags = FF16PackChunkCompressionType.UseSpecificChunk;
                    task.PackFile.IsCompressed = true;
                }
            }

            ArrayPool<byte>.Shared.Return(compBuffer);
        }
        else
        {
            // File will only fit using multiple chunks
            Console.WriteLine($"PACK: Compressing '{task.GamePath}' into multiple chunks..");

            using var fileStream = File.Open(task.LocalPath, FileMode.Open);
            long remBytes = fileStream.Length;

            long startDataOffset = packStream.Position;
            long lastDataOffset = startDataOffset;

            task.PackFile.DataOffset = (ulong)packStream.Position;
            task.PackFile.ChunkDefOffset = (ulong)_lastMultiChunkHeaderOffset;

            var crc = new Crc32();
            byte[] buffer = ArrayPool<byte>.Shared.Rent(0x80000);
            byte[] compBuffer = ArrayPool<byte>.Shared.Rent(0x100000); // double, incase it's somehow larger

            int i = 0;
            uint totalCompressedSize = 0;
            while (remBytes > 0)
            {
                int thisSize = (int)Math.Min(remBytes, 0x80000);
                fileStream.Read(buffer, 0, thisSize);
                crc.Append(buffer.AsSpan(0, thisSize));

                packStream.Position = (long)task.PackFile.ChunkDefOffset + 8 + (i * sizeof(int));
                task.SplitChunkOffsets[i] = (uint)(lastDataOffset - startDataOffset);

                packStream.Position = lastDataOffset;
                unsafe
                {
                    fixed (byte* inputDecompChunkPtr = buffer)
                    fixed (byte* outputCompChunkPtr = compBuffer)
                    {
                        CompressBuffer(_codec, (nint)inputDecompChunkPtr, (long)thisSize,
                            Compression.BestRatio, // Matches original
                            (nint)outputCompChunkPtr, (long)0x100000, out long compressedDataSize);

                        packStream.Write(compBuffer, 0, (int)compressedDataSize);
                        lastDataOffset = packStream.Position;

                        totalCompressedSize += (uint)compressedDataSize;
                    }
                }

                if (i == task.SplitChunkOffsets.Length - 1)
                    task.SplitLastChunkSize = (uint)remBytes;

                remBytes -= thisSize;
                i++;
            }

            task.PackFile.CRC32Checksum = crc.GetCurrentHashAsUInt32();
            task.PackFile.IsCompressed = true;
            task.PackFile.ChunkedCompressionFlags = FF16PackChunkCompressionType.UseMultipleChunks;
            task.PackFile.CompressedFileSize = totalCompressedSize;

            long chunkHeaderSize = (long)8 + (task.SplitChunkOffsets.Length * sizeof(int));
            task.PackFile.ChunkHeaderSize = (uint)chunkHeaderSize;
            _lastMultiChunkHeaderOffset += chunkHeaderSize;

        }
    }

    public void WriteSharedChunk(FileStream packStream, ChunkTask chunkTask)
    {
        byte[] decompBuffer = ArrayPool<byte>.Shared.Rent((int)chunkTask.PackChunk.DecompressedSize);
        long sizeCompressed = _codec.CompressBufferBound(chunkTask.PackChunk.DecompressedSize);
        byte[] compBuffer = ArrayPool<byte>.Shared.Rent((int)sizeCompressed);

        using var bufferStream = new MemoryStream(decompBuffer);
        foreach (var file in chunkTask.Files)
        {
            bufferStream.Position = (long)file.PackFile.DataOffset;
            using var inputFileStream = File.Open(file.LocalPath, FileMode.Open);
            uint crc = CopyToWithChecksum(inputFileStream, bufferStream);

            file.PackFile.CRC32Checksum = crc;
            file.PackFile.CompressedFileSize = (uint)file.PackFile.DecompressedFileSize;
            file.PackFile.IsCompressed = true;
            file.PackFile.ChunkedCompressionFlags = FF16PackChunkCompressionType.UseSharedChunk;
            file.PackFile.ChunkHeaderSize = FF16PackDStorageChunk.GetSize();
        }

        unsafe
        {
            fixed (byte* inputDecompChunkPtr = decompBuffer)
            fixed (byte* outputCompChunkPtr = compBuffer)
            {
                CompressBuffer(_codec, (nint)inputDecompChunkPtr, chunkTask.PackChunk.DecompressedSize, 
                    Compression.BestRatio, // Matches original
                    (nint)outputCompChunkPtr, chunkTask.PackChunk.DecompressedSize, out long compressedDataSize);

                chunkTask.PackChunk.DataOffset = (ulong)packStream.Position;
                chunkTask.PackChunk.CompressedChunkSize = (uint)compressedDataSize;
                packStream.Write(compBuffer, 0, (int)compressedDataSize);
            }
        }

        ArrayPool<byte>.Shared.Return(decompBuffer);
        ArrayPool<byte>.Shared.Return(compBuffer);
    }

    private static uint CopyToWithChecksum(Stream input, Stream output)
    {
        var crc = new Crc32();
        byte[] buffer = new byte[0x20000];

        long len = input.Length;
        while (len > 0)
        {
            int thisSize = (int)Math.Min(len, 0x20000);
            input.Read(buffer, 0, thisSize);
            output.Write(buffer, 0, thisSize);
            crc.Append(buffer.AsSpan(0, thisSize));
            len -= thisSize;
        }

        return crc.GetCurrentHashAsUInt32();
    }

    // This is a hack. CompressBuffer would offer no way to grab back the compressed size
    private static unsafe void CompressBuffer(IDStorageCompressionCodec codec,
        nint uncompressedData, PointerSize uncompressedDataSize, Compression compressionSetting, nint compressedBuffer, PointerSize compressedBufferSize, out long compressedDataSize)
    {
        long value;
        long** vtbl = (long**)codec.NativePointer;
        ((Result)((delegate* unmanaged[Stdcall]<nint, void*, void*, int, void*, void*, void*, int>)(*vtbl)[3])(
            codec.NativePointer, (void*)uncompressedData, uncompressedDataSize, (int)compressionSetting, (void*)compressedBuffer, compressedBufferSize, &value)).CheckError();
        compressedDataSize = value;
    }

    public bool IsCompressionForFileSuggested(string file)
    {
        string ext = Path.GetExtension(file);
        switch (ext)
        {
            // 0000
            case ".anmb":
            case ".tex":
            case ".mdl":
            case ".spd8":
            case ".tera":
            case ".gid":
            case ".gtex":
            case ".epb":
            case ".sab": // <- Some, but not all, are decompressed. 
            case ".mab":

            // 0003
            case ".bk2": // Bink video
                return false;
        }

        return true;
    }
}

public class ChunkTask
{
    public List<FileTask> Files { get; set; } = [];
    public FF16PackDStorageChunk PackChunk { get; set; } = new();
}

public class FileTask
{
    public string LocalPath { get; set; }
    public string GamePath { get; set; }
    public FF16PackFile PackFile { get; set; } = new();
    public long StringTableNameOffsetRelative { get; set; }
    public ChunkTask SharedChunk { get; set; }
    public uint[] SplitChunkOffsets { get; set; }
    public uint SplitLastChunkSize { get; set; }
}