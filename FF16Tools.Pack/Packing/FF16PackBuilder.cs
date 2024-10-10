using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Buffers;

using Microsoft.Extensions.Logging;

using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.HighPerformance;

using Vortice.DirectStorage;

using SharpGen.Runtime;

using Syroot.BinaryData.Memory;
using Syroot.BinaryData;

using FF16Tools.Hashing;
using FF16Tools.Crypto;
using FF16Tools.Shared;

namespace FF16Tools.Pack.Packing;

public class FF16PackBuilder
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private List<FileTask> _packFileTasks { get; set; } = [];
    private List<ChunkTask> _sharedChunksTasks { get; set; } = [];
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

    public FF16PackBuilder(PackBuildOptions options = null, ILoggerFactory loggerFactory = null)
    {
        _options = options ?? new();
        _loggerFactory = loggerFactory;

        if (_loggerFactory is not null)
            _logger = _loggerFactory.CreateLogger(GetType().ToString());
    }

    /// <summary>
    /// Inits the builder from the specified directory to pack.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="ct"></param>
    public void InitFromDirectory(string dir, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dir, nameof(dir));

        dir = Path.TrimEndingDirectorySeparator(dir).Replace('\\', '/');
        if (string.IsNullOrEmpty(_options.Name) && File.Exists(Path.Combine(dir, ".path")))
        {
            RegisterPathFile(Path.Combine(dir, ".path"));
        }

        if (!string.IsNullOrEmpty(_options.Name) && !Directory.Exists(Path.Combine(dir, _options.Name)))
        {
            throw new DirectoryNotFoundException($"Directory '{_options.Name}' does not exist inside input directory.");
        }

        List<string> fileList = [];
        foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
        {
            fileList.Add(file.Replace('\\', '/'));
            ct.ThrowIfCancellationRequested();
        }

        dir = dir.Replace("\\", "/");

        var files = fileList.Order().ToList();
        foreach (var file in files)
        {
            string gamePath = file[(dir.Length + 1)..].ToLower();
            if (Path.GetFileName(file) == ".path")
                continue;

            AddFile(file, gamePath);
        }
    }

    /// <summary>
    /// Registers a .path file. This should be called first (if it exists) before adding files.
    /// </summary>
    /// <param name="path"></param>
    public void RegisterPathFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            _logger?.LogWarning(".path file should have two lines, but it has {lineCount}.", lines.Length);
        }
        else
        {
            _logger?.LogInformation("Using archive name/dir '{dir}' from .path file", lines[1]);
            _options.Name = lines[1].Replace('\\', '/');
        }
    }

    /// <summary>
    /// Adds a file to the pack.
    /// </summary>
    /// <param name="localPath"></param>
    /// <param name="gamePath"></param>
    public void AddFile(string localPath, string gamePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(localPath, nameof(gamePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath, nameof(gamePath));

        gamePath = FF16PackPathUtil.NormalizePath(gamePath);

        if (!string.IsNullOrEmpty(_options.Name))
        {
            if (!gamePath.StartsWith(_options.Name))
                throw new ArgumentException($"Game path should start with '{_options.Name}'.");

            gamePath = Path.GetRelativePath(_options.Name, gamePath);
        }

        gamePath = FF16PackPathUtil.NormalizePath(gamePath); // normalize again since GetRelativePath can swap back to '\\'..

        _logger?.LogInformation("PACK: Adding '{path}'...", gamePath);

        var fileInfo = new FileInfo(localPath);
        var task = new FileTask()
        {
            LocalPath = localPath,
            GamePath = gamePath,
        };
        task.PackFile.DecompressedFileSize = (ulong)fileInfo.Length;

        _packFileTasks.Add(task);
    }

    private void BuildSharedChunks()
    {
        int i = 0;
        foreach (var file in _packFileTasks)
        {
            if (!IsCompressionForFileSuggested(file.GamePath) || file.PackFile.DecompressedFileSize == 0)
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
                _logger?.LogInformation("PACK: Compressing '{path}' into shared chunk..", file.GamePath);
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

    /// <summary>
    /// Writes the pack to the specified file.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public async Task WriteToAsync(string file, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));

        BuildSharedChunks();
        ct.ThrowIfCancellationRequested();
        BuildStringTable();
        ct.ThrowIfCancellationRequested();

        _logger?.LogInformation("PACK: Starting write.");

        using var fs = new FileStream(file, FileMode.Create);
        uint headerLength = CalculateHeaderSize();

        _lastMultiChunkHeaderOffset = FF16Pack.HEADER_SIZE + (uint)(_packFileTasks.Count * FF16PackFile.GetSize());

        // Start writing the files first
        fs.Position = (long)headerLength;

        for (int i = 0; i < _sharedChunksTasks.Count; i++)
        {
            _logger?.LogInformation("PACK: Writing shared chunk {chunkNumber}/{totalChunks}..", i + 1, _sharedChunksTasks.Count);
            ChunkTask chunk = _sharedChunksTasks[i];
            await WriteSharedChunk(fs, chunk, ct);
        }

        foreach (FileTask task in _packFileTasks)
        {
            if (task.SharedChunk is not null)
                continue; // Already written

            await WriteFile(fs, task, ct);
        }

        byte[] header = new byte[(int)headerLength];
        WriteHeader(fs, header);
    }

    private void WriteHeader(FileStream packStream, byte[] headerBuffer)
    {
        _logger?.LogInformation("PACK: Writing header.");

        if (_options.Encrypt)
            _logger?.LogInformation("PACK: Header encryption is enabled.");
        if (!string.IsNullOrEmpty(_options.Name))
            _logger?.LogInformation("PACK: Setting internal pack name to '{name}'.", _options.Name);

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
        Encoding.UTF8.GetBytes(_options.Name ?? "", nameBuffer);
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

    private async Task WriteFile(FileStream packStream, FileTask task, CancellationToken ct = default)
    {
        if (!IsCompressionForFileSuggested(task.GamePath) || task.PackFile.DecompressedFileSize == 0)
        {
            _logger?.LogInformation("PACK: Writing raw '{path}'..", task.GamePath);

            task.PackFile.DataOffset = (ulong)packStream.Position;

            using var fileStream = File.Open(task.LocalPath, FileMode.Open);
            uint crc = await CopyToWithChecksumAsync(fileStream, packStream, ct);
            task.PackFile.CRC32Checksum = crc;
            task.PackFile.CompressedFileSize = (uint)task.PackFile.DecompressedFileSize;
        }
        else if ((long)task.PackFile.DecompressedFileSize < FF16Pack.MIN_FILE_SIZE_FOR_MULTIPLE_CHUNKS)
        {
            _logger?.LogInformation("PACK: Compressing '{path}' into unique chunk..", task.GamePath);

            task.PackFile.DataOffset = (ulong)packStream.Position;

            using MemoryOwner<byte> decompBuffer = MemoryOwner<byte>.Allocate((int)task.PackFile.DecompressedFileSize);
            long sizeCompressed = _codec.CompressBufferBound((long)task.PackFile.DecompressedFileSize * 2); // Incase
            using MemoryOwner<byte> compBuffer = MemoryOwner<byte>.Allocate((int)sizeCompressed);

            using var fileStream = new FileStream(task.LocalPath, FileMode.Open);
            using var decompStream = decompBuffer.AsStream();
            uint crc = await CopyToWithChecksumAsync(fileStream, decompStream, ct);

            uint compressedDataSize = GDeflate.Compress(
                decompBuffer.Span.Slice(0, (int)task.PackFile.DecompressedFileSize), compBuffer.Span);

            await packStream.WriteAsync(compBuffer.Memory.Slice(0, (int)compressedDataSize), ct);

            task.PackFile.CRC32Checksum = crc;
            task.PackFile.CompressedFileSize = compressedDataSize;
            task.PackFile.ChunkedCompressionFlags = FF16PackChunkCompressionType.UseSpecificChunk;
            task.PackFile.IsCompressed = true;
        }
        else
        {
            // File will only fit using multiple chunks
            _logger?.LogInformation("PACK: Compressing '{path}' into multiple chunks..", task.GamePath);

            using var fileStream = File.Open(task.LocalPath, FileMode.Open);
            long remBytes = fileStream.Length;

            long startDataOffset = packStream.Position;
            long lastDataOffset = startDataOffset;

            task.PackFile.DataOffset = (ulong)packStream.Position;
            task.PackFile.ChunkDefOffset = (ulong)_lastMultiChunkHeaderOffset;

            var crc = new Crc32();
            using MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate(0x80000);
            using MemoryOwner<byte> compBuffer = MemoryOwner<byte>.Allocate(0x100000); // double, incase it's somehow larger

            int i = 0;
            uint totalCompressedSize = 0;
            while (remBytes > 0)
            {
                int thisSize = (int)Math.Min(remBytes, 0x80000);

                Memory<byte> slice = buffer.Memory.Slice(0, thisSize);
                await fileStream.ReadAsync(slice, ct);
                crc.Append(slice.Span);

                packStream.Position = (long)task.PackFile.ChunkDefOffset + 8 + (i * sizeof(int));
                task.SplitChunkOffsets[i] = (uint)(lastDataOffset - startDataOffset);

                packStream.Position = lastDataOffset;
                uint compressedDataSize = GDeflate.Compress(slice.Span, compBuffer.Span);

                await packStream.WriteAsync(compBuffer.Memory.Slice(0, (int)compressedDataSize), ct);
                lastDataOffset = packStream.Position;

                totalCompressedSize += (uint)compressedDataSize;

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

    private async Task WriteSharedChunk(FileStream packStream, ChunkTask chunkTask, CancellationToken ct = default)
    {
        using MemoryOwner<byte> decompBuffer = MemoryOwner<byte>.Allocate((int)chunkTask.PackChunk.DecompressedSize);
        long sizeCompressed = _codec.CompressBufferBound(chunkTask.PackChunk.DecompressedSize);
        using MemoryOwner<byte> compBuffer = MemoryOwner<byte>.Allocate((int)sizeCompressed);

        using var bufferStream = decompBuffer.AsStream();
        foreach (var file in chunkTask.Files)
        {
            bufferStream.Position = (long)file.PackFile.DataOffset;
            using var inputFileStream = File.Open(file.LocalPath, FileMode.Open);
            uint crc = await CopyToWithChecksumAsync(inputFileStream, bufferStream, ct);

            file.PackFile.CRC32Checksum = crc;
            file.PackFile.CompressedFileSize = (uint)file.PackFile.DecompressedFileSize;
            file.PackFile.IsCompressed = true;
            file.PackFile.ChunkedCompressionFlags = FF16PackChunkCompressionType.UseSharedChunk;
            file.PackFile.ChunkHeaderSize = FF16PackDStorageChunk.GetSize();
        }

        uint compressedDataSize = GDeflate.Compress(
            decompBuffer.Span.Slice(0, (int)chunkTask.PackChunk.DecompressedSize), compBuffer.Span);

        chunkTask.PackChunk.DataOffset = (ulong)packStream.Position;
        chunkTask.PackChunk.CompressedChunkSize = compressedDataSize;
        await packStream.WriteAsync(compBuffer.Memory.Slice(0, (int)compressedDataSize), ct);
    }

    private static async Task<uint> CopyToWithChecksumAsync(FileStream inputFile, Stream output, CancellationToken ct = default)
    {
        var crc = new Crc32();
        using MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate(0x20000);

        long len = inputFile.Length;
        while (len > 0)
        {
            int thisSize = (int)Math.Min(len, 0x20000);
            Memory<byte> slice = buffer.Memory.Slice(0, thisSize);
            await inputFile.ReadAsync(slice, ct);
            await output.WriteAsync(slice, ct);
            crc.Append(slice.Span);
            len -= thisSize;
        }

        return crc.GetCurrentHashAsUInt32();
    }

    public bool IsCompressionForFileSuggested(string file)
    {
        string ext = Path.GetExtension(file);
        switch (ext)
        {
            // 0000
            case ".anmb":
            case ".tex": // <- Textures, texture data is already dstorage compressed.
            case ".mdl": // <- Models, geometry is already dstorage compressed.
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
    public ChunkTask SharedChunk { get; set; }
    public uint[] SplitChunkOffsets { get; set; }
    public uint SplitLastChunkSize { get; set; }
}