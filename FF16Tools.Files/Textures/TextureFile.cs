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
        if (texture.Dimension == 3)
            throw new NotSupportedException("3D textures are not supported yet.");

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

                if (chunkInfo.CompressedChunkSize == chunkInfo.DecompressedChunkSize) // TODO: Find the "is compressed" flag
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

    /* TODO: Restore this when it's eventually figured out.
     * Paint.NET seems to give literally no crap to the pitch field in dds (so does anything else)
     * Textures seem to be padded, so it's important if we're just shoving the data straight into a dds
     * doesn't work though, bleh. */
    /*
    public MemoryOwner<byte> GetAsDds(int textureIndex, Stream textureFileStream)
    {
        using var buffer = GetTextureData(textureIndex, textureFileStream);
        var texture = Textures[textureIndex];
        DXGI_FORMAT dxgiFormat = TextureUtils.TexPixelFormatToDdsFormat(texture.PixelFormat);

        var ddsHeader = MemoryOwner<byte>.Allocate(0x100 + buffer.Length, AllocationMode.Clear);

        int padded = (int)AlignValue(texture.Width, 64);

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
            PitchOrLinearSize = (int)AlignValue(texture.Width, 64),
            DxgiFormat = TextureUtils.TexPixelFormatToDdsFormat(texture.PixelFormat),
            ImageData = buffer.Memory,
        };

        dds.Write(ddsHeader.AsStream());
        return ddsHeader;
    }
    */

    public Image GetImageData(int textureIndex, Stream fileStream)
    {
        long basePos = fileStream.Position;

        using MemoryOwner<byte> textureData = GetTextureData(textureIndex, fileStream);

        var texture = Textures[textureIndex];

        int paddedWidth = (int)Align(texture.Width, 64);
        if (texture.SignedDistanceField) // Seems to be the only way this works (used for font textures), so w/e i guess
            paddedWidth = (int)Align(texture.Width, 256); 

        int paddedHeight = texture.Height > 4 ? (int)Align(texture.Height, 4) : texture.Height;

        var bytesPerPixel = DdsHeader.BitsPerPixel(TextureUtils.TexPixelFormatToDdsFormat(texture.PixelFormat)) / 8;
        int pixelSize = paddedWidth * paddedHeight * bytesPerPixel;
        byte[] pixelBuffer = new byte[pixelSize];
        textureData.Span.Slice(0, Math.Min(pixelSize, textureData.Length)).CopyTo(pixelBuffer); // Decompressed size does not mean it's the actual size of the pixel buffer

        Image img;
        switch (texture.PixelFormat) // as of 24/08/2024, this is all the formats used by the demo
        {
            case TexturePixelFormat.R8_UNORM:
                img = Image.LoadPixelData<L8>(pixelBuffer, paddedWidth, texture.Height);
                break;

            case TexturePixelFormat.R16_UINT:
            case TexturePixelFormat.R16_UNORM:
                img = Image.LoadPixelData<L16>(pixelBuffer, paddedWidth, texture.Height);
                break;

            case TexturePixelFormat.R16_FLOAT:
                img = Image.LoadPixelData<HalfSingle>(pixelBuffer, paddedWidth, texture.Height);
                break;

            case TexturePixelFormat.R16G16B16A16_FLOAT:
                img = Image.LoadPixelData<HalfVector4>(pixelBuffer, paddedWidth, texture.Height);
                break;

            case TexturePixelFormat.R8G8B8A8_UNORM:
            case TexturePixelFormat.R8G8B8A8_UNORM_SRGB:
                img = Image.LoadPixelData<Rgba32>(pixelBuffer, paddedWidth, texture.Height);
                break;

            case TexturePixelFormat.BC3_UNORM:
            case TexturePixelFormat.BC4_UNORM:
            case TexturePixelFormat.BC5_UNORM:
            case TexturePixelFormat.BC6H_SF16:
            case TexturePixelFormat.BC6H_UF16:
            case TexturePixelFormat.BC7_UNORM:
            case TexturePixelFormat.BC7_UNORM_SRGB:
                {
                    var memStream = textureData.Memory.AsStream();

                    var decoder = new BcDecoder();

                    var bcType = texture.PixelFormat switch
                    {
                        TexturePixelFormat.BC3_UNORM => BCnEncoder.Shared.CompressionFormat.Bc3,
                        TexturePixelFormat.BC4_UNORM => BCnEncoder.Shared.CompressionFormat.Bc4,
                        TexturePixelFormat.BC5_UNORM => BCnEncoder.Shared.CompressionFormat.Bc5,
                        TexturePixelFormat.BC6H_UF16 or TexturePixelFormat.BC6H_SF16 => throw new NotSupportedException("HDR format (BC6H_UF16) is not yet supported."),
                        TexturePixelFormat.BC7_UNORM or TexturePixelFormat.BC7_UNORM_SRGB => BCnEncoder.Shared.CompressionFormat.Bc7,
                        _ => throw new NotSupportedException(),
                    };

                    img = decoder.DecodeRawToImageRgba32(memStream, paddedWidth, paddedHeight, bcType);
                }
                break;

            // Known to be used, not yet supported.
            case TexturePixelFormat.R32G32_UINT:
            case TexturePixelFormat.R10G10B10A2_UNORM:
                throw new NotSupportedException($"{texture.PixelFormat} is not yet supported.");

            default:
                throw new NotSupportedException($"{texture.PixelFormat} is not yet supported.");
        }

        img.Mutate(e => e.Crop(texture.Width, texture.Height));
        fileStream.Position = 0;

        return img;
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
