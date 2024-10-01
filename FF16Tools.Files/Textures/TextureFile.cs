using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using Syroot.BinaryData;

using FF16Tools.Shared;

using FF16Tools.Shared.Dds;
using BCnEncoder.ImageSharp;
using BCnEncoder.Decoder;

namespace FF16Tools.Files.Textures;

/// <summary>
/// FF16 '.tex' texture file.
/// </summary>
public class TextureFile
{
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    /// <summary>
    /// Magic, 'TEX '
    /// </summary>
    public const uint MAGIC = 0x20584554;
    public const uint HEADER_SIZE = 0x28;

    public byte Version { get; set; }
    public byte Flags { get; set; }
    public byte UnkHeader_0x06 { get; set; }

    public uint FileFlags2 { get; set; }

    public List<TextureInfo> Textures { get; set; } = [];
    public List<TextureChunkInfo> TextureChunks { get; set; } = [];

    public TextureFile(ILoggerFactory loggerFactory = null)
    {
        _loggerFactory = loggerFactory;

        if (_loggerFactory is not null)
            _logger = _loggerFactory.CreateLogger(GetType().ToString());
    }

    public void FromStream(Stream stream)
    {
        var bs = new BinaryStream(stream, ByteConverter.Little);

        ReadMainHeader(bs);
        ReadTextureHeader(bs);
    }

    public MemoryOwner<byte> GetTextureData(int textureIndex, Stream textureFileStream)
    {
        long basePos = textureFileStream.Position;

        var texture = Textures[textureIndex];

        // Used by: 
        // - system/graphics/atmosphere/texture/tmultiscattering_atms.tex (R16G16B16A16_FLOAT)
        if (texture.DimensionType == 2)
            throw new NotSupportedException("3D textures (type 2) are not supported yet.");

        // Used by:
        // - system/graphics/texture/omni_cube_index.tex
        if (texture.DimensionType == 3)
            throw new NotSupportedException("3D textures (type 3) are not supported yet.");

        _logger?.LogInformation("Texture #{texIndex}: {width}x{height} ({format})", textureIndex, texture.Width, texture.Height, texture.PixelFormat);

        if (!texture.NoChunks)
        {
            int size = 0;
            for (int i = texture.ChunkIndex; i < texture.ChunkIndex + texture.ChunkCount; i++)
                size += (int)TextureChunks[i].DecompressedChunkSize;

            MemoryOwner<byte> decompressedBuffer = MemoryOwner<byte>.Allocate(size);

            int offset = 0;
            for (int i = 0; i < texture.ChunkCount; i++)
            {
                TextureChunkInfo chunkInfo = TextureChunks[texture.ChunkIndex + i];
                textureFileStream.Position = basePos + chunkInfo.CompressedDataOffset;

                _logger?.LogTrace("Extracting texture chunk #{index}, offset @ 0x{offset:X8}, compSize=0x{compSize:X8}, decompSize=0x{decompSize:X8}", 
                    texture.ChunkIndex + i, chunkInfo.CompressedDataOffset, chunkInfo.CompressedChunkSize, chunkInfo.DecompressedChunkSize);

                if (chunkInfo.CompressedChunkSize == chunkInfo.DecompressedChunkSize) // <- This is how the game checks
                {
                    textureFileStream.Read(decompressedBuffer.Span.Slice(offset, (int)chunkInfo.DecompressedChunkSize));
                }
                else
                {
                    using MemoryOwner<byte> chunkBuffer = MemoryOwner<byte>.Allocate((int)chunkInfo.CompressedChunkSize);
                    textureFileStream.Read(chunkBuffer.Span);
                    GDeflate.Decompress(chunkBuffer.Span, decompressedBuffer.Span.Slice(offset, (int)chunkInfo.DecompressedChunkSize));
                }

                offset += (int)chunkInfo.DecompressedChunkSize;
            }

            return decompressedBuffer;
        }
        else
        {
            MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate((int)texture.DataSize);

            textureFileStream.Position = basePos + texture.DataOffset;
            textureFileStream.Read(buffer.Span);

            return buffer;
        }
    }


    public MemoryOwner<byte> GetAsDds(int textureIndex, Stream textureFileStream)
    {
        using var inputData = GetTextureData(textureIndex, textureFileStream);
        var texture = Textures[textureIndex];
        DXGI_FORMAT dxgiFormat = TextureUtils.TexPixelFormatToDdsFormat(texture.PixelFormat);

        uint totalSize = 0;
        int w = texture.Width; int h = texture.Height;

        // Start calculating the total size we need for our output DDS
        // Note: While it is possible to determine the total size of a texture using alignedSlicePitch
        // >> The last row of the last mip is never padded to D3D12_TEXTURE_DATA_PITCH_ALIGNMENT. <<

        ulong basePitch = 0;
        for (int i = 0; i < texture.MipmapCount; i++)
        {
            DxgiUtils.ComputePitch(dxgiFormat, w, h, out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch);
            totalSize += (uint)slicePitch;

            w >>= 1;
            h >>= 1;

            if (i == 0)
                basePitch = rowPitch;
        }

        using MemoryOwner<byte> nonAlignedData = MemoryOwner<byte>.Allocate((int)totalSize);
        MemoryOwner<byte> ddsHeader = MemoryOwner<byte>.Allocate(0x100 + nonAlignedData.Length, AllocationMode.Clear);

        w = texture.Width;
        h = texture.Height;
        ulong offset = 0, alignedOffset = 0;
        for (int i = 0; i < texture.MipmapCount; i++)
        {
            DxgiUtils.ComputePitch(dxgiFormat, w, h, out ulong rowPitch, out ulong slicePitch, out ulong alignedSlicePitch);

            // Very important. Each row is padded to 256 bytes normally (only if dstorage chunked, which will pass the data directly into a ID3D12Resource).
            // Non-chunked:
            // - gracommon/texture/speedtree/
            // - system/graphics/atmosphere/ (note: the textures are way larger than they should be?)
            uint rowPitchAligned = texture.ChunkCount == 0 ? (uint)rowPitch : Align((uint)rowPitch, DxgiUtils.D3D12_TEXTURE_DATA_PITCH_ALIGNMENT);
            uint thisPitchSize = i == texture.MipmapCount - 1 ? (uint)rowPitch : (uint)alignedSlicePitch;

            Span<byte> inputMip = inputData.Span.Slice((int)alignedOffset);
            Span<byte> outputMip = nonAlignedData.Span.Slice((int)offset, (int)slicePitch);

            // Gotta divide by 4 for BC formats as it deals in 4x4 blocks.
            int actualHeight = DxgiUtils.IsBCnFormat(dxgiFormat) ? h / 4 : h;
            for (int y = 0; y < actualHeight; y++)
            {
                Span<byte> inputRow = inputMip.Slice((int)(y * rowPitchAligned), (int)rowPitch);
                Span<byte> outputRow = outputMip.Slice((int)(y * (uint)rowPitch), (int)rowPitch);
                inputRow.CopyTo(outputRow);
            }

            w >>= 1;
            h >>= 1;

            offset += slicePitch;
            alignedOffset += thisPitchSize;
        }

        var flags = DDSHeaderFlags.TEXTURE | DDSHeaderFlags.LINEARSIZE;
        if (texture.MipmapCount > 1)
            flags |= DDSHeaderFlags.MIPMAP;

        var dds = new DdsHeader()
        {
            Flags = flags,
            Width = texture.Width,
            Height = texture.Height,
            FormatFlags = DDSPixelFormatFlags.DDPF_FOURCC,
            LastMipmapLevel = texture.MipmapCount,
            FourCCName = "DX10",
            PitchOrLinearSize = (int)basePitch,
            DxgiFormat = TextureUtils.TexPixelFormatToDdsFormat(texture.PixelFormat),
            ImageData = nonAlignedData.Memory,
        };

        dds.Write(ddsHeader.AsStream());
        return ddsHeader;
    }
    
    private void ReadMainHeader(BinaryStream bs)
    {
        Debug.Assert(bs.Length - bs.Position >= HEADER_SIZE, "Not enough space for texture header");

        if (bs.ReadUInt32() != MAGIC)
            throw new NotSupportedException();

        Version = bs.Read1Byte();
        Flags = bs.Read1Byte();
        UnkHeader_0x06 = bs.Read1Byte();
        bs.ReadByte(); // 1 byte padding
    }

    private void ReadTextureHeader(BinaryStream bs)
    {
        byte textureCount = bs.Read1Byte();
        bs.Read1Byte();
        ushort numChunks = bs.ReadUInt16();
        bs.ReadUInt32();
        bs.ReadUInt32();
        FileFlags2 = bs.ReadUInt32();

        bs.Position = HEADER_SIZE;
        for (int i = 0; i < textureCount; i++)
        {
            bs.Position = HEADER_SIZE + (i * TextureInfo.GetSize());
            var texture = new TextureInfo();
            texture.FromStream(bs);
            Textures.Add(texture);
        }

        bs.Position = HEADER_SIZE + (Textures.Count * TextureInfo.GetSize());
        for (int i = 0; i < numChunks; i++)
        {
            var chunkInfo = new TextureChunkInfo();
            chunkInfo.FromStream(bs);
            TextureChunks.Add(chunkInfo);
        }
    }

    private static uint Align(uint x, uint alignment)
    {
        uint mask = ~(alignment - 1);
        return (x + (alignment - 1)) & mask;
    }
}
