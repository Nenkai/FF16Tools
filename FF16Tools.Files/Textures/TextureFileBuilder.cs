using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using BCnEncoder.Shared.ImageFiles;

using CommunityToolkit.HighPerformance.Buffers;

using FF16Tools.Shared;
using FF16Tools.Shared.Dds;

using Microsoft.Extensions.Logging;

using Pfim;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

using Syroot.BinaryData;

namespace FF16Tools.Files.Textures;

public class TextureFileBuilder
{
    private readonly ILoggerFactory? _loggerFactory;
    private readonly ILogger? _logger;

    public const int MAX_TEXTURES = 255;
    private readonly List<TextureBuildTask> _textureBuildTasks = [];

    private long _textureInfosOffset;
    private long _chunksOffset;
    private long _lastDataOffset;

    private uint _lastChunkIndex;

    public TextureFileBuilder(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory;

        if (_loggerFactory is not null)
            _logger = _loggerFactory.CreateLogger(GetType().ToString());
    }

    public void AddImage(string fileName, TextureOptions? options = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));

        Debug.Assert(_textureBuildTasks.Count + 1 < MAX_TEXTURES, "AddImage: Texture file cannot hold more than 255 textures");

        TextureBuildTask task;
        string ext = Path.GetExtension(fileName);
        if (ext == ".dds")
        {
            task = ProcessDds(fileName);
        }
        else if (ext == ".png" || ext == ".jpg" || ext == ".gif" || ext == ".tga" || ext == ".bmp" || ext == ".webp")
        {
            task = ProcessOther(fileName);
        }
        else
            throw new NotSupportedException($"Input image format '{ext}' is not supported.");

        if (options is not null)
        {
            task.Options = options;

            if (task.Options.NoChunkCompression)
                _logger?.LogInformation("NoChunkCompression provided: Chunks won't be compressed.");

            if (task.Options.NoChunks)
                _logger?.LogInformation("NoChunks provided: Textures won't be chunked for DirectStorage.");

            if (task.Options.SignedDistanceField)
                _logger?.LogInformation("SignedDistanceField provided: Texture is signed distance field.");
        }
    }

    private TextureBuildTask ProcessDds(string fileName)
    {
        _logger?.LogInformation("Processing dds file '{fileName}'...", fileName);

        using var fs = new FileStream(fileName, FileMode.Open);
        var header = new Pfim.DdsHeader(fs);

        // https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide
        DXGI_FORMAT format = DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM;
        if (header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.DX10))
        {
            var dx10Header = new DdsHeaderDxt10(fs);
            format = (DXGI_FORMAT)dx10Header.DxgiFormat;
        }
        else if (header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.D3DFMT_DXT1))
            format = DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
        else if (header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.D3DFMT_DXT2) || header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.D3DFMT_DXT3))
            format = DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM;
        else if (header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.D3DFMT_DXT4) || header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.D3DFMT_DXT5))
            format = DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
        else if (header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.BC4U))
            format = DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
        else if (header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.BC4S))
            format = DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM;
        else if (header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.ATI2) || header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.BC5U))
            format = DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
        else if (header.PixelFormat.FourCC.HasFlag(CompressionAlgorithm.BC5S))
            format = DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM;
        else
            throw new NotSupportedException($"Dds FourCC {header.PixelFormat.FourCC} is not supported.");

        _logger?.LogInformation("DDS Header:");
        _logger?.LogInformation("- FourCC: {fcc}", header.PixelFormat.FourCC);
        _logger?.LogInformation("- Dimensions: {width}x{height}", header.Width, header.Height);
        _logger?.LogInformation("- Mipmaps: {mipmapCount}", header.MipMapCount);
        _logger?.LogInformation("- Format: {format}", format);
        _logger?.LogInformation("- Depth: {depth}", header.Depth);

        Debug.Assert(header.Width <= ushort.MaxValue, "ProcessDds: Dds width too large");
        Debug.Assert(header.Height <= ushort.MaxValue, "ProcessDds: Dds height too large");

        var textureTask = new TextureBuildTask((ushort)header.Width, (ushort)header.Height, TextureUtils.DxgiFormatToTexPixelFormat(format));

        uint width = header.Width;
        uint height = header.Height;
        for (int i = 0; i < header.MipMapCount; i++)
        {
            DxgiUtils.ComputePitch(format, width, height, out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch);

            byte[] data = fs.ReadBytes((int)slicePitch);
            textureTask.Mipmaps.Add(data);

            width >>= 1;
            height >>= 1;

            _logger?.LogTrace("Dds Mipmap #{i} - {width}x{height}, size 0x{size:X}", i, width, height, slicePitch);
        }

        _textureBuildTasks.Add(textureTask);
        return textureTask;
    }

    private TextureBuildTask ProcessOther(string fileName)
    {
        _logger?.LogInformation("Processing standard file '{fileName}'... (will be using R8G8B8A8_UNORM)", fileName);

        var img = Image.Load<Rgba32>(fileName);

        var textureTask = new TextureBuildTask((ushort)img.Width, (ushort)img.Height, TexturePixelFormat.R8G8B8A8_UNORM);

        DxgiUtils.ComputePitch(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, (uint)img.Width, (uint)img.Height, out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch);
        byte[] buffer = new byte[slicePitch];
        for (int y = 0; y < img.Height; y++)
        {
            var pixels = img.DangerousGetPixelRowMemory(y);
            ReadOnlySpan<byte> rowBytes = MemoryMarshal.Cast<Rgba32, byte>(pixels.Span);
            rowBytes.CopyTo(buffer.AsSpan(y * (int)rowPitch));
        }
        textureTask.Mipmaps.Add(buffer);
        _textureBuildTasks.Add(textureTask);

        return textureTask;
    }

    public void Build(Stream outputStream)
    {
        _lastChunkIndex = 0;

        _logger?.LogInformation("Calculating number of chunks...");

        int numChunks = 0;
        for (int i = 0; i < _textureBuildTasks.Count; i++)
        {
            if (_textureBuildTasks[i].Options.NoChunks)
                continue;

            if (_textureBuildTasks[i].Mipmaps.Count > 1)
                numChunks += 2;
            else
                numChunks++;
        }

        Debug.Assert(numChunks <= ushort.MaxValue, "Too many chunks in texture building");

        _logger?.LogInformation("Chunks: {chunkCount}", numChunks);
        _logger?.LogInformation("Writing header...");

        using var bs = new BinaryStream(outputStream, ByteConverter.Little);
        bs.Write("TEX "u8);
        bs.WriteByte(4); // Latest version
        bs.WriteByte(1); // Flags
        bs.WriteByte(11); // Unknown
        bs.WriteByte(0);

        // Header 2
        bs.WriteByte((byte)_textureBuildTasks.Count);
        bs.WriteByte(0);
        bs.WriteUInt16((ushort)numChunks);
        bs.WriteUInt32(0);
        bs.WriteUInt32(0);
        bs.WriteUInt32(0x03);
        bs.Position += sizeof(int) * 4;

        _textureInfosOffset = bs.Position;
        _chunksOffset = _textureInfosOffset + (_textureBuildTasks.Count * 0x20);
        _lastDataOffset = _chunksOffset + (numChunks * 0x10);
        if (numChunks > 0)
            _lastDataOffset = TextureUtils.Align((uint)_lastDataOffset, 0x10);

        uint currentChunkIndex = 0;

        // Do chunks
        for (int i = 0; i < _textureBuildTasks.Count; i++)
        {
            TextureBuildTask task = _textureBuildTasks[i];
            if (task.Options.NoChunks)
                continue;

            Debug.Assert(task.Mipmaps.Count >= 1, "Texture has no mipmaps or data at all?");

            _logger?.LogInformation("Writing texture #{i} chunked ({width}x{height} {format})...", i, task.Width, task.Height, task.Format);
            WriteFileChunked(bs, ref currentChunkIndex, task);
        }

        _logger?.LogInformation("Writing texture infos ({count})", _textureBuildTasks.Count);

        for (int i = 0; i < _textureBuildTasks.Count; i++)
        {
            bs.Position = _textureInfosOffset + (i * 0x20);

            TextureBuildTask task = _textureBuildTasks[i];
            uint bits = (uint)(task.DimensionType & 0b11) |
                        (task.Options.SignedDistanceField ? 1u : 0u) << 2 |
                        (task.Options.NoChunks ? 1u : 0u) << 3 |
                        0xFFFFFFu << 8;
            bs.WriteUInt32(bits);
            bs.WriteUInt32((uint)task.Format);
            bs.WriteUInt16((ushort)task.Mipmaps.Count);
            bs.WriteUInt16(task.Width);
            bs.WriteUInt16(task.Height);
            bs.WriteUInt16(task.Depth);
            bs.WriteUInt32(task.ChunkOffset);
            bs.WriteUInt32(task.CompressedDataSize);
            bs.WriteUInt32(0xFFFFFFFF); // Color
            bs.WriteUInt16(task.ChunkIndex);
            bs.WriteUInt16(task.ChunkCount);
        }

        bs.Position = _lastDataOffset;
    }

    private void WriteFileChunked(BinaryStream bs, ref uint currentChunkIndex, TextureBuildTask task)
    {
        DXGI_FORMAT dxgiFormat = TextureUtils.TexPixelFormatToDxgiFormat(task.Format);

        // Calculate required buffer size
        ulong totalAlignedSize = 0;
        uint width = task.Width;
        uint height = task.Height;
        ulong[] alignedMipSizes = new ulong[task.Mipmaps.Count];
        for (int j = 0; j < task.Mipmaps.Count; j++)
        {
            DxgiUtils.ComputePitch(dxgiFormat, width, height, out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch);

            alignedMipSizes[j] = alignedSlicePitch;
            totalAlignedSize += alignedSlicePitch;

            width >>= 1;
            height >>= 1;
        }

        // Actually start writing
        width = task.Width;
        height = task.Height;
        MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate((int)totalAlignedSize);
        ulong offset = 0;

        // Process through all the mipmaps and align them
        for (int j = 0; j < task.Mipmaps.Count; j++)
        {
            DxgiUtils.ComputePitch(dxgiFormat, width, height, out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch);
            uint rowPitchAligned = task.Options.NoChunks ? (uint)rowPitch : TextureUtils.Align((uint)rowPitch, DxgiUtils.D3D12_TEXTURE_DATA_PITCH_ALIGNMENT);
            uint actualHeight = DxgiUtils.IsBCnFormat(dxgiFormat) ? height / 4 : height;

            Span<byte> outputMip = buffer.Span.Slice((int)offset, (int)alignedSlicePitch);

            for (int y = 0; y < actualHeight; y++)
            {
                Memory<byte> inputRow = task.Mipmaps[j].Slice((y * (int)rowPitch), (int)rowPitch);
                Span<byte> outputRow = outputMip.Slice((int)(y * rowPitchAligned), (int)rowPitch);
                inputRow.Span.CopyTo(outputRow);
            }

            width >>= 1;
            height >>= 1;

            offset += alignedSlicePitch;
        }

        task.ChunkIndex = (ushort)_lastChunkIndex;
        task.ChunkOffset = (uint)_lastDataOffset;

        bs.Position = _lastDataOffset;

        // Create chunk for the main mip

        var firstMipData = buffer.Span.Slice(0, (int)alignedMipSizes[0]);
        task.CompressedDataSize += WriteChunk(bs, firstMipData, !task.Options.NoChunkCompression);
        task.ChunkCount = 1;

        // All other mips fit into a singular chunk.
        if (task.Mipmaps.Count > 1)
        {
            var restMipData = buffer.Span.Slice((int)alignedMipSizes[0]);
            task.CompressedDataSize += WriteChunk(bs, restMipData, !task.Options.NoChunkCompression);
            task.ChunkCount = 2;
        }
    }

    private uint WriteChunk(BinaryStream bs, Span<byte> toCompress, bool useCompression)
    {
        uint compressedSize = (uint)toCompress.Length;
        Span<byte> outputChunk = toCompress;
        MemoryOwner<byte>? compressedMemOwner = null;
        if (useCompression)
        {
            compressedMemOwner = MemoryOwner<byte>.Allocate(toCompress.Length);
            outputChunk = compressedMemOwner.Span;
            compressedSize = GDeflate.Compress(toCompress, outputChunk);
        }

        bs.Position = _chunksOffset + (_lastChunkIndex * 0x10);
        WriteChunkInfo(bs, 0, _lastDataOffset, _lastChunkIndex, (uint)toCompress.Length, compressedSize);

        bs.Position = _lastDataOffset;
        bs.Write(outputChunk.Slice(0, (int)compressedSize));
        _lastDataOffset = bs.Position;

        _lastChunkIndex++;

        compressedMemOwner?.Dispose();
        return compressedSize;
    }

    private void WriteChunkInfo(BinaryStream bs, byte typeFlags, long dataOffset, uint chunkIndex, uint decompressedSize, uint compressedSize)
    {
        Debug.Assert(typeFlags <= 3, "WriteChunkInfo: Dimension type above 3?");

        _logger?.LogInformation("Chunk: offset=0x{dataOffset:X}, type={type}, decompressedSize={decompressedSize:X}, compressedSize={compressedSize:X}",
            dataOffset, typeFlags, decompressedSize, compressedSize);

        bs.WriteUInt32((uint)dataOffset);
        bs.WriteUInt32((uint)(typeFlags & 0b11) | (compressedSize << 2));
        bs.WriteUInt32(decompressedSize);
        bs.WriteByte((byte)(chunkIndex & 0b1111111 | 0));
        bs.WriteByte(0);
        bs.Position += 2; // Pad
    }
}

class TextureBuildTask
{
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public TexturePixelFormat Format { get; set; }
    public List<Memory<byte>> Mipmaps { get; set; } = [];
    public ushort Depth { get; set; } = 1;

    public byte DimensionType = 1;

    public TextureOptions Options { get; set; } = new();

    public uint ChunkOffset { get; set; }
    public uint CompressedDataSize { get; set; }
    public ushort ChunkIndex { get; set; }
    public ushort ChunkCount { get; set; }

    public TextureBuildTask(ushort width, ushort height, TexturePixelFormat dxgiFormat)
    {
        Width = width;
        Height = height;
        Format = dxgiFormat;
    }
}

public class TextureOptions
{
    public bool NoChunks { get; set; } = false;
    public bool SignedDistanceField { get; set; } = false;
    public bool NoChunkCompression { get; set; } = false;
}
