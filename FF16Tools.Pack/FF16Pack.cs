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
using CommunityToolkit.HighPerformance;

using Syroot.BinaryData;

using FF16Tools.Hashing;
using FF16Tools.Crypto;
using FF16Tools.Shared;

namespace FF16Tools.Pack;

/// <summary>
/// FF16 Pack file. (Disposable object)
/// </summary>
public class FF16Pack : IDisposable, IAsyncDisposable
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    public const uint MAGIC = 0x4B434150;
    public const uint HEADER_SIZE = 0x400;

    public const int MIN_FILE_SIZE_FOR_MULTIPLE_CHUNKS = 0x2000000;
    public const int MAX_FILE_SIZE_FOR_SHARED_CHUNK = 0x100000;
    public const int MAX_DECOMPRESSED_SHARED_CHUNK_SIZE = 0x400000;
    public const int MAX_DECOMPRESSED_MULTI_CHUNK_SIZE = 0x80000;

    /// <summary>
    /// Internal Archive/Parent directory for this pack.
    /// </summary>
    public string ArchiveDir { get; private set; }

    /// <summary>
    /// Whether the header is/was encrypted.
    /// </summary>
    public bool HeaderEncrypted { get; set; }

    /// <summary>
    /// Whether the pack uses shared (?) chunks.
    /// </summary>
    public bool UsesChunks { get; set; }

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

    /// <summary>
    /// Opens a pack file.
    /// </summary>
    /// <param name="path">Pack path.</param>
    /// <param name="loggerFactory">Logger factory, for logging.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the file is not a pack file.</exception>
    public static FF16Pack Open(string path, ILoggerFactory loggerFactory = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

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
            pack.UsesChunks = bs.ReadBoolean();
            pack.HeaderEncrypted = bs.ReadBoolean();
            ushort numChunks = bs.ReadUInt16();
            ulong packSize = bs.ReadUInt64();
            bs.Position = 0x18;
            if (pack.HeaderEncrypted)
                XorEncrypt.CryptHeaderPart(header.AsSpan(0x18, 0x100));
            pack.ArchiveDir = bs.ReadString(StringCoding.ZeroTerminated);

            bs.Position = 0x118;
            ulong chunksTableOffset = bs.ReadUInt64();
            ulong stringsOffset = bs.ReadUInt64();
            ulong stringTableSize = bs.ReadUInt64();

            if (pack.HeaderEncrypted)
                XorEncrypt.CryptHeaderPart(header.AsSpan((int)stringsOffset, (int)stringTableSize));

            for (int i = 0; i < numFiles; i++)
            {
                bs.Position = HEADER_SIZE + (i * FF16PackFile.GetSize());
                var file = new FF16PackFile();
                file.FromStream(bs);

                bs.Position = (long)file.FileNameOffset;
                string fileName = bs.ReadString(StringCoding.ZeroTerminated);
                pack._files.Add(Path.Combine(pack.ArchiveDir, fileName).Replace('\\', '/'), file);

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

    /// <summary>
    /// Gets a file info for the specified path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">If the file is not found in the archive.</exception>
    public FF16PackFile GetFileInfo(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        path = path.Replace('\\', '/');
        if (!_files.TryGetValue(path, out FF16PackFile file))
            throw new FileNotFoundException("File not found in pack.");

        return file;
    }

    /// <summary>
    /// Gets the number of files in this pack.
    /// </summary>
    /// <returns></returns>
    public int GetNumFiles()
    {
        return _files.Count;
    }

    /// <summary>
    /// Gets the number of shared chunks in this pack.
    /// </summary>
    /// <returns></returns>
    public int GetNumSharedChunks()
    {
        return _chunks.Count;
    }

    /// <summary>
    /// Returns whether the specified file exists in this pack.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool FileExists(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        return _files.ContainsKey(path);
    }

    private int? _fileCounter;

    /// <summary>
    /// Gets file data to a stream.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">If the file is not found in the archive.</exception>
    public async Task GetFileDataStream(string path, Stream outputStream, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentNullException.ThrowIfNull(outputStream);

        if (!outputStream.CanWrite)
            throw new ArgumentException("Output stream should be writable.");

        FF16PackFile file = GetFileInfo(path);
        await UnpackFileToStream(file, outputStream, path, ct: ct);
    }

    /// <summary>
    /// Gets file data. Returning buffer is disposable.
    /// </summary>
    /// <param name="path">Game path.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">If the file is not found in the archive.</exception>
    public async Task<MemoryOwner<byte>> GetFileData(string path, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        FF16PackFile file = GetFileInfo(path);

        MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate((int)file.DecompressedFileSize);
        await UnpackFileToStream(file, buffer.AsStream(), path, ct: ct);
        return buffer;
    }

    /// <summary>
    /// Extracts all the files in the pack to the specified directory.
    /// </summary>
    /// <param name="outputDir">Output directory for the extracted files.</param>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
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

        if (!string.IsNullOrEmpty(ArchiveDir))
        {
            File.WriteAllLines(Path.Combine(outputDir, ".path"), 
            [ 
                "// This is the internal name/dir for this pack. Do not rename it if you are repacking.",
                ArchiveDir
            ]);
        }
    }

    /// <summary>
    /// Extracts the specified file from the pack to the specified output directory.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="outputDir"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task ExtractFile(string path, string outputDir, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentException.ThrowIfNullOrEmpty(outputDir);

        if (!_files.TryGetValue(path, out FF16PackFile packFile))
            throw new FileNotFoundException("File not found in pack.");

        if (_fileCounter is not null)
            _logger?.LogInformation("[{fileNumber}/{fileCount}] Extracting '{path}' (0x{packSize:X} bytes)...", _fileCounter + 1, _files.Count, path, packFile.DecompressedFileSize);
        else
            _logger?.LogInformation("Extracting '{path}' (0x{packSize:X} bytes)...", path, packFile.DecompressedFileSize);

        string outputPath = Path.Combine(Path.GetFullPath(outputDir), ArchiveDir, path);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        var outputStream = new FileStream(outputPath, FileMode.Create);
        await UnpackFileToStream(packFile, outputStream, outputPath, ct);
    }

    private async Task UnpackFileToStream(FF16PackFile packFile, Stream outputStream, string pathForLogging, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching {path} (compressed={compressed}, dataOffset=0x{offset:X8})", pathForLogging, packFile.IsCompressed, packFile.DataOffset);

        if (packFile.IsCompressed)
        {
            _logger.LogTrace("Compression type: {compressionType}", packFile.ChunkedCompressionFlags);
            switch (packFile.ChunkedCompressionFlags)
            {
                case FF16PackChunkCompressionType.UseSharedChunk:
                    await ExtractFileFromSharedChunk(packFile, outputStream, pathForLogging, ct);
                    break;
                case FF16PackChunkCompressionType.UseMultipleChunks:
                    await ExtractFileFromMultipleChunks(packFile, outputStream, pathForLogging, ct);
                    break;
                case FF16PackChunkCompressionType.UseSpecificChunk:
                    await ExtractFileFromSpecificChunk(packFile, outputStream, pathForLogging, ct);
                    break;
                case FF16PackChunkCompressionType.None:
                    throw new ArgumentException($"Pack file '{pathForLogging}' has compression flag but compression type flag is '{packFile.ChunkedCompressionFlags}'..?");
                default:
                    throw new NotSupportedException($"Compression type {packFile.ChunkedCompressionFlags} for file '{pathForLogging}'");
            }
        }
        else
        {
            int size = (int)packFile.DecompressedFileSize;
            _stream.Position = (long)packFile.DataOffset;

            using MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate(0x20000);

            var crc = new Crc32();
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
                ThrowHashException(pathForLogging);
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

    private async ValueTask ExtractFileFromMultipleChunks(FF16PackFile packFile, Stream outputStream, string outputPath, CancellationToken ct = default)
    {
        _stream.Position = (long)packFile.ChunkDefOffset;

        using MemoryOwner<byte> compBuffer = MemoryOwner<byte>.Allocate(0x100000);
        using MemoryOwner<byte> decompBuffer = MemoryOwner<byte>.Allocate(MAX_DECOMPRESSED_MULTI_CHUNK_SIZE);

        uint numChunks = _stream.ReadUInt32();
        uint size = _stream.ReadUInt32();
        uint[] chunkOffsets = _stream.ReadUInt32s((int)numChunks);

        var crc = new Crc32();
        long remSize = (long)packFile.DecompressedFileSize;
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

    private async ValueTask ExtractFileFromSpecificChunk(FF16PackFile packFile, Stream outputStream, string outputPath, CancellationToken ct = default)
    {
        _stream.Position = (long)packFile.DataOffset;

        using MemoryOwner<byte> compBuffer = MemoryOwner<byte>.Allocate((int)packFile.CompressedFileSize);
        using MemoryOwner<byte> decompBuffer = MemoryOwner<byte>.Allocate((int)packFile.DecompressedFileSize);
        Memory<byte> compSlice = compBuffer.Memory.Slice(0, (int)packFile.CompressedFileSize);
        Memory<byte> decompSlice = decompBuffer.Memory.Slice(0, (int)packFile.DecompressedFileSize);

        await _stream.ReadAsync(compSlice, ct);

        GDeflate.Decompress(compSlice.Span, decompBuffer.Span.Slice(0, MAX_DECOMPRESSED_MULTI_CHUNK_SIZE));

        await outputStream.WriteAsync(decompSlice, ct);

        uint hash = Crc32.HashToUInt32(decompSlice.Span);
        if (hash != packFile.CRC32Checksum)
            ThrowHashException(outputPath);
    }

    private async ValueTask ExtractFileFromSharedChunk(FF16PackFile packFile, Stream outputStream, string gamePath, CancellationToken ct = default)
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

        await outputStream.WriteAsync(chunk.CachedBuffer.AsMemory((int)packFile.DataOffset, (int)packFile.DecompressedFileSize), ct);

        uint hash = Crc32.HashToUInt32(chunk.CachedBuffer.AsSpan((int)packFile.DataOffset, (int)packFile.DecompressedFileSize));
        if (hash != packFile.CRC32Checksum)
            ThrowHashException(gamePath);

        _cachedChunks.Add(chunk);
    }

    public void DumpInfo()
    {
        _logger?.LogInformation($"Pack Info:");
        _logger?.LogInformation("- Internal Archive Name/Dir: {name}", (string.IsNullOrEmpty(ArchiveDir) ? "(none)" : ArchiveDir));
        _logger?.LogInformation("- Num Files: {numFiles}", GetNumFiles());
        _logger?.LogInformation("- Chunks: {numChunks}", GetNumSharedChunks());
        _logger?.LogInformation("- Header Encryption: {headerEncrypted}", HeaderEncrypted);
        _logger?.LogInformation("- Uses Chunks: {useChunks}", UsesChunks);
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

        ((IDisposable)_stream)?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var cachedChunk in _cachedChunks)
        {
            if (cachedChunk.CachedBuffer is not null)
                ArrayPool<byte>.Shared.Return(cachedChunk.CachedBuffer);
        }

        if (_stream is not null)
            await _stream.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
