using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO.Hashing;
using SysDebug = System.Diagnostics.Debug;

using Microsoft.Extensions.Logging;

using CommunityToolkit.HighPerformance.Buffers;

using Vortice.DirectStorage;
using Syroot.BinaryData;

using FF16Tools.Hashing;
using FF16Tools.Crypto;
using CommunityToolkit.HighPerformance;


namespace FF16Tools.Pack;

/// <summary>
/// FF16 Pack file. (Disposable object)
/// </summary>
public class FF16Pack : IDisposable
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    public const uint MAGIC = 0x4B434150;
    public const uint HEADER_SIZE = 0x400;

    public const int MIN_FILE_SIZE_FOR_MULTIPLE_CHUNKS = 0x2000000;
    public const int MAX_FILE_SIZE_FOR_SHARED_CHUNK = 0x100000;
    public const int MAX_DECOMPRESSED_SHARED_CHUNK_SIZE = 0x400000;
    public const int MAX_DECOMPRESSED_MULTI_CHUNK_SIZE = 0x80000;

    public string ArchiveDir { get; private set; }
    public bool HeaderEncrypted { get; set; }
    public bool UseChunks { get; set; }

    private Dictionary<string, FF16PackFile> _files = [];
    private List<FF16PackDStorageChunk> _chunks { get; set; } = [];
    private Dictionary<long, FF16PackDStorageChunk> _offsetToChunk = [];

    private FileStream _stream;
    private HashSet<FF16PackDStorageChunk> _cachedChunks = [];

    private FF16Pack(FileStream stream, ILoggerFactory loggerFactory = null)
    {
        _stream = stream;
        _loggerFactory = loggerFactory;

        if (_loggerFactory is not null)
            _logger = _loggerFactory.CreateLogger(GetType().ToString());
    }

    public static FF16Pack Open(string path, ILoggerFactory loggerFactory = null)
    {
        var fs = File.OpenRead(path);
        var fileBinStream = new BinaryStream(fs);

        if (fileBinStream.ReadUInt32() != MAGIC)
            throw new InvalidDataException("Not a FF16 Pack file");

        FF16Pack pack = new FF16Pack(fs, loggerFactory);
        uint headerSize = fileBinStream.ReadUInt32();
        fileBinStream.Position = 0;
        byte[] header = fileBinStream.ReadBytes((int)headerSize);

        using (var ms = new MemoryStream(header))
        using (var bs = new BinaryStream(ms))
        {
            bs.Position = 8;
            uint numFiles = bs.ReadUInt32();
            pack.UseChunks = bs.ReadBoolean();
            pack.HeaderEncrypted = bs.ReadBoolean();
            ushort numChunks = bs.ReadUInt16();
            ulong packSize = bs.ReadUInt64();
            bs.Position = 0x18;

            if (pack.HeaderEncrypted)
                XorEncrypt.CryptHeaderPart(header.AsSpan(0x18, 0x100));

            pack.ArchiveDir = Encoding.UTF8.GetString(header.AsSpan(0x18, 0x100)).TrimEnd('\0');

            bs.Position = 0x118;
            ulong chunksTableOffset = bs.ReadUInt64();
            ulong stringsOffset = bs.ReadUInt64();
            ulong stringTableSize = bs.ReadUInt64();

            if (pack.HeaderEncrypted)
                XorEncrypt.CryptHeaderPart(header.AsSpan((int)stringsOffset, (int)stringTableSize));

            for (int i = 0; i < numFiles; i++)
            {
                bs.Position = 0x400 + (i * 0x38);
                var file = new FF16PackFile();
                file.FromStream(bs);

                bs.Position = (long)file.FileNameOffset;
                string fileName = bs.ReadString(StringCoding.ZeroTerminated);
                pack._files.Add(fileName, file);

                SysDebug.Assert(Fnv1Hash.HashPath(fileName) == file.FileNameHash, $"File name hash did not match ({fileName})");
            }

            pack._chunks.Capacity = numChunks;
            for (uint i = 0; i < numChunks; i++)
            {
                bs.Position = (long)(chunksTableOffset + (i * (ulong)FF16PackDStorageChunk.GetSize()));
                var chunk = new FF16PackDStorageChunk();
                pack._offsetToChunk.Add(bs.Position, chunk);

                chunk.FromStream(bs);
                pack._chunks.Add(chunk);
            }
        }

        return pack;
    }

    public int GetNumFiles()
    {
        return _files.Count;
    }

    public int GetNumChunks()
    {
        return _chunks.Count;
    }

    public bool FileExists(string path)
    {
        return _files.ContainsKey(path);
    }

    private int? _fileCounter;
    public async Task ExtractAll(string outputDir)
    {
        if (string.IsNullOrEmpty(outputDir))
            throw new DirectoryNotFoundException("Output dir is invalid.");

        _fileCounter = 0;
        foreach (KeyValuePair<string, FF16PackFile> file in _files)
        {
            await ExtractFile(file.Key, outputDir);
            _fileCounter++;
        }
    }

    public async Task ExtractFile(string path, string outputDir, CancellationToken ct = default)
    {
        if (!_files.TryGetValue(path, out FF16PackFile packFile))
            throw new FileNotFoundException("File not found in pack.");

        if (_fileCounter is not null)
            _logger?.LogInformation("[{fileNumber}/{fileCount}] Extracting '{path}' (0x{packSize:X} bytes)...", _fileCounter + 1, _files.Count, path, packFile.DecompressedFileSize);
        else
            _logger?.LogInformation("Extracting '{path}' (0x{packSize:X} bytes)...", path, packFile.DecompressedFileSize);

        string outputPath = Path.Combine(Path.GetFullPath(outputDir), path);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        if (packFile.IsCompressed)
        {
            switch (packFile.ChunkedCompressionFlags)
            {
                case FF16PackChunkCompressionType.UseSharedChunk:
                    await ExtractFileFromSharedChunk(packFile, outputPath, ct);
                    break;
                case FF16PackChunkCompressionType.UseMultipleChunks:
                    await ExtractFileFromMultipleChunks(packFile, outputPath, ct);
                    break;
                case FF16PackChunkCompressionType.UseSpecificChunk:
                    await ExtractFileFromSpecificChunk(packFile, outputPath, ct);
                    break;
                case FF16PackChunkCompressionType.None:
                    throw new ArgumentException($"Pack file '{path}' has compression flag but compression type flag is '{packFile.ChunkedCompressionFlags}'..?");
                default:
                    throw new NotSupportedException($"Compression type {packFile.ChunkedCompressionFlags} for file '{path}'");
            }
        }
        else
        {
            int size = (int)packFile.DecompressedFileSize;
            _stream.Position = (long)packFile.DataOffset;

            using MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate(0x20000);

            var crc = new Crc32();
            using var outputStream = new FileStream(outputPath, FileMode.Create);
            while (size > 0)
            {
                int cnt = Math.Min(size, buffer.Length);
                Memory<byte> slice = buffer.Memory.Slice(0, cnt);

                await _stream.ReadAsync(slice, ct);
                await outputStream.WriteAsync(slice, ct);

                crc.Append(slice.Span);
                size -= cnt;

                ct.ThrowIfCancellationRequested();
            }

            if (crc.GetCurrentHashAsUInt32() != packFile.CRC32Checksum)
                ThrowHashException(outputPath);
        }
    }

    public void ListFiles(string outputPath, bool log = false)
    {
        using var sw = new StreamWriter(outputPath);
        foreach (var file in _files)
        {
            sw.WriteLine($"{file.Key} - crc:{file.Value.CRC32Checksum:X8}, nameHash:{file.Value.FileNameHash:X8} compressed: {file.Value.IsCompressed} ({file.Value.ChunkedCompressionFlags}), " +
                $"dataOffset: 0x{file.Value.DataOffset:X16}, fileSize: 0x{file.Value.DecompressedFileSize:X8}, compressedFileSize: 0x{file.Value.CompressedFileSize:X8}, chunkHeaderSize:0x{file.Value.ChunkHeaderSize:X8}");

            if (log)
            {
                _logger?.LogInformation("{path} - crc:{crc:X8}, nameHash:{nameHash:X8} compressed: {isCompressed} ({compressionFlags}), " +
                    "dataOffset: 0x{dataOffset:X16}, fileSize: 0x{decompressedFileSize:X8}, compressedFileSize: 0x{compressedSize:X8}, chunkHeaderSize:0x{chunkHeaderSize:X8}",
                    file.Key,
                    file.Value.CRC32Checksum,
                    file.Value.FileNameHash,
                    file.Value.IsCompressed,
                    file.Value.ChunkedCompressionFlags,
                    file.Value.DataOffset,
                    file.Value.DecompressedFileSize,
                    file.Value.CompressedFileSize,
                    file.Value.ChunkHeaderSize);
            }
        }
    }

    private async ValueTask ExtractFileFromMultipleChunks(FF16PackFile packFile, string outputPath, CancellationToken ct = default)
    {
        _stream.Position = (long)packFile.ChunkDefOffset;

        using MemoryOwner<byte> compBuffer = MemoryOwner<byte>.Allocate(0x100000);
        using MemoryOwner<byte> decompBuffer = MemoryOwner<byte>.Allocate(MAX_DECOMPRESSED_MULTI_CHUNK_SIZE);

        uint numChunks = _stream.ReadUInt32();
        uint size = _stream.ReadUInt32();
        uint[] chunkOffsets = _stream.ReadUInt32s((int)numChunks);

        var crc = new Crc32();
        long remSize = (long)packFile.DecompressedFileSize;
        var outputStream = new FileStream(outputPath, FileMode.Create);
        for (int i = 0; i < numChunks; i++)
        {
            int chunkCompSize = i < numChunks - 1 ?
                  (int)(chunkOffsets[i + 1] - chunkOffsets[i])
                : (int)packFile.CompressedFileSize - (int)chunkOffsets[i];
            int chunkDecompSize = (int)Math.Min(remSize, MAX_DECOMPRESSED_MULTI_CHUNK_SIZE);

            _stream.Position = (long)(packFile.DataOffset + chunkOffsets[i]);
            Memory<byte> compSlice = compBuffer.Memory.Slice(0, chunkCompSize);
            Memory<byte> decompSlice = decompBuffer.Memory.Slice(0, chunkDecompSize);
            await _stream.ReadAsync(compSlice, ct);

            GDeflate.Decompress(compSlice.Span, decompSlice.Span);

            await outputStream.WriteAsync(decompSlice, ct);
            crc.Append(decompSlice.Span);
            remSize -= chunkDecompSize;
        }

        if (crc.GetCurrentHashAsUInt32() != packFile.CRC32Checksum)
            ThrowHashException(outputPath);
    }

    private async ValueTask ExtractFileFromSpecificChunk(FF16PackFile packFile, string outputPath, CancellationToken ct = default)
    {
        _stream.Position = (long)packFile.DataOffset;

        using MemoryOwner<byte> compBuffer = MemoryOwner<byte>.Allocate((int)packFile.CompressedFileSize);
        using MemoryOwner<byte> decompBuffer = MemoryOwner<byte>.Allocate((int)packFile.DecompressedFileSize);
        Memory<byte> compSlice = compBuffer.Memory.Slice(0, (int)packFile.CompressedFileSize);
        Memory<byte> decompSlice = decompBuffer.Memory.Slice(0, (int)packFile.DecompressedFileSize);

        await _stream.ReadAsync(compSlice, ct);

        GDeflate.Decompress(compSlice.Span, decompBuffer.Span.Slice(0, MAX_DECOMPRESSED_MULTI_CHUNK_SIZE));

        using var outputStream = new FileStream(outputPath, FileMode.Create);
        await outputStream.WriteAsync(decompSlice, ct);

        uint hash = Crc32.HashToUInt32(decompSlice.Span);
        if (hash != packFile.CRC32Checksum)
            ThrowHashException(outputPath);
    }

    private async ValueTask ExtractFileFromSharedChunk(FF16PackFile packFile, string outputPath, CancellationToken ct = default)
    {
        FF16PackDStorageChunk chunk = _offsetToChunk[(long)packFile.ChunkDefOffset];
        if (!_cachedChunks.Contains(chunk))
        {
            if (_cachedChunks.Count >= 20)
            {
                var chunkToRemove = _cachedChunks.First();
                ArrayPool<byte>.Shared.Return(chunkToRemove.CachedBuffer);
                chunkToRemove.CachedBuffer = null;

                _cachedChunks.Remove(chunkToRemove);
            }
        }

        _stream.Position = (long)chunk.DataOffset;

        if (chunk.CachedBuffer is null)
        {
            using MemoryOwner<byte> compBuffer = MemoryOwner<byte>.Allocate((int)chunk.CompressedChunkSize);
            Memory<byte> compSlice = compBuffer.Memory.Slice(0, (int)chunk.CompressedChunkSize);

            byte[] decompressedBuffer = ArrayPool<byte>.Shared.Rent((int)chunk.DecompressedSize);

            try
            {
                await _stream.ReadAsync(compSlice, ct);

                GDeflate.Decompress(compSlice.Span,
                    decompressedBuffer.AsSpan(0, (int)chunk.DecompressedSize));

                chunk.CachedBuffer = decompressedBuffer;
            }
            catch (Exception)
            {
                if (chunk.CachedBuffer is not null)
                    ArrayPool<byte>.Shared.Return(decompressedBuffer);

                throw;
            }
        }

        using var outputStream = new FileStream(outputPath, FileMode.Create);
        await outputStream.WriteAsync(chunk.CachedBuffer.AsMemory((int)packFile.DataOffset, (int)packFile.DecompressedFileSize), ct);

        uint hash = Crc32.HashToUInt32(chunk.CachedBuffer.AsSpan((int)packFile.DataOffset, (int)packFile.DecompressedFileSize));
        if (hash != packFile.CRC32Checksum)
            ThrowHashException(outputPath);

        _cachedChunks.Add(chunk);
    }

    public void DumpInfo()
    {
        _logger?.LogInformation($"Pack Info:");
        _logger?.LogInformation("- Internal Archive Name/Dir: {name}", (string.IsNullOrEmpty(ArchiveDir) ? "(none)" : ArchiveDir));
        _logger?.LogInformation("- Num Files: {numFiles}", GetNumFiles());
        _logger?.LogInformation("- Chunks: {numChunks}", GetNumChunks());
        _logger?.LogInformation("- Header Encryption: {headerEncrypted}", HeaderEncrypted);
        _logger?.LogInformation("- Uses Chunks: {useChunks}", UseChunks);
    }

    private static void ThrowHashException(string path)
    {
        throw new InvalidDataException($"Hash for file '{path}' did not match.");
    }

    public void Dispose()
    {
        foreach (var cachedChunk in _cachedChunks)
        {
            if (cachedChunk.CachedBuffer is not null)
                ArrayPool<byte>.Shared.Return(cachedChunk.CachedBuffer);
        }

        ((IDisposable)_stream).Dispose();
        GC.SuppressFinalize(this);
    }
}
