using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Textures;

/// <summary>
/// Represents a texture inside a texture file.
/// </summary>
public class TextureInfo
{
    // 2 bit type (dimension?)
    // 1 bit signed distance field texture
    // 1 bit no chunks
    // 1 bit unk
    // 1 bit unk
    // rest of bits I have no idea

    /// <summary>
    /// Raw bit flags for this texture.
    /// </summary>
    public uint BitFlags { get; set; }

    /// <summary>
    /// Unknown (1 to 3)
    /// </summary>
    public byte Dimension
    {
        get => (byte)(BitFlags & 0b11);
        set => BitFlags |= (uint)(value & 0b11);
    }

    /// <summary>
    /// Whether this texture uses signed distance field (usually for fonts, but also used for some other textures like masks).
    /// </summary>
    public bool SignedDistanceField
    {
        get => ((BitFlags >> 2) & 0b1) == 1;
        set => BitFlags |= (uint)((value ? 1 : 0)) << 2;
    }

    /// <summary>
    /// Whether this texture does not use chunks.
    /// </summary>
    public bool NoChunks
    {
        get => ((BitFlags >> 3) & 1) == 1;
        set => BitFlags |= (uint)((value ? 1 : 0) << 3);
    }

    /// <summary>
    /// Pixel format of this texture.
    /// </summary>
    public TexturePixelFormat PixelFormat { get; set; }

    /// <summary>
    /// Number of mips in this texture.
    /// </summary>
    public ushort MipmapCount { get; set; }

    /// <summary>
    /// Original width of this texture.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    /// Original height of this texture.
    /// </summary>
    public ushort Height { get; set; }

    public ushort Field_0x0E { get; set; }

    /// <summary>
    /// Offset to the start of the data for this texture.
    /// </summary>
    public uint DataOffset { get; set; }

    /// <summary>
    /// Data size in the file (compressed, or not)
    /// </summary>
    public uint DataSize { get; set; }

    /// <summary>
    /// Unknown. Average color? Fill color?
    /// </summary>
    public uint ColorMaybe { get; set; }

    /// <summary>
    /// Number of compressed chunks used by this texture (if compressed)
    /// </summary>
    public ushort ChunkCount { get; set; }

    /// <summary>
    /// Start compessed chunk index for this texture
    /// </summary>
    public ushort ChunkIndex { get; set; }

    public void FromStream(BinaryStream bs)
    {
        BitFlags = bs.ReadUInt32();
        PixelFormat = (TexturePixelFormat)bs.ReadUInt32();
        MipmapCount = bs.ReadUInt16();
        Width = bs.ReadUInt16();
        Height = bs.ReadUInt16();
        Field_0x0E = bs.ReadUInt16();
        DataOffset = bs.ReadUInt32();
        DataSize = bs.ReadUInt32();
        ColorMaybe = bs.ReadUInt32();
        ChunkIndex = bs.ReadUInt16();
        ChunkCount = bs.ReadUInt16();
    }

    public static int GetSize()
    {
        return 0x20;
    }
}

// sub_1409B3764 - ConvertDdsToCustomFormat (FF16 Demo 1.0.0)
// sub_1409B4464 - ConvertCustomToDdsFormat
// sub_1409B4084
// XXYYYY - XX = Group, YYYY = Format
// There's probably more. Only followed that function which translates dds format to custom
public enum TexturePixelFormat
{
    // 8 bpp
    R8_TYPELESS = 0x10130,
    R8_UNORM = 0x11130,
    A8_UNORM = 0x11131,
    R8_SNORM = 0x13130,
    R8_UINT = 0x14130,
    R8_SINT = 0x15130,

    // 16 bpp
    R16_TYPELESS = 0x20140,
    R16_UNORM = 0x21140,
    R16_SNORM = 0x23140,
    R16_UINT = 0x24140,
    R16_SINT = 0x25140,
    R16_FLOAT = 0x26140,
    D16_UNORM = 0x29140,

    // 8+8 bpp
    R8G8_TYPELESS = 0x30240,
    R8G8_UNORM = 0x31240,
    R8G8_UINT = 0x34240,
    R8G8_SNORM = 0x33240,
    R8G8_SINT = 0x35240,

    // 32 bpp
    R32_TYPELESS = 0x40150,
    R32_UINT = 0x44150,
    R32_SINT = 0x45150,
    R32_FLOAT = 0x46150,
    D32_FLOAT = 0x49150,

    // 16+16 bpp
    R16G16_TYPELESS = 0x50250,
    R16G16_UNORM = 0x51250,
    R16G16_SNORM = 0x53250,
    R16G16_UINT = 0x54250,
    R16G16_SINT = 0x55250,
    R16G16_FLOAT = 0x56250,

    // 11+11+10 bpp
    R11G11B10_FLOAT = 0x76350,

    // 11+10+10+2 bpp
    R10G10B10A2_TYPELESS = 0x80450,
    R10G10B10A2_UNORM = 0x81450,
    R10G10B10A2_UINT = 0x84450,

    // 8+8+8+8 bpp
    R8G8B8A8_TYPELESS = 0xA0450,
    R8G8B8A8_UNORM = 0xA1450,
    R8G8B8A8_UNORM_SRGB = 0xA2450,
    R8G8B8A8_UINT = 0xA4450,
    R8G8B8A8_SNORM = 0xA3450,
    R8G8B8A8_SINT = 0xA5450,

    // 32+32 bpp
    R32G32_TYPELESS = 0xB0260,
    R32G32_FLOAT = 0xB6260,
    R32G32_UINT = 0xB4260,
    R32G32_SINT = 0xB5260,

    // 16+16+16+16 bpp
    R16G16B16A16_TYPELESS = 0xC0460,
    R16G16B16A16_FLOAT = 0xC6460,
    R16G16B16A16_UNORM = 0xC1460,
    R16G16B16A16_UINT = 0xC4460,
    R16G16B16A16_SNORM = 0xC3460,
    R16G16B16A16_SINT = 0xC5460,

    // 32+32+32 bpp
    R32G32B32_TYPELESS = 0xD0380,
    R32G32B32_FLOAT = 0xD6380,
    R32G32B32_UINT = 0xD4380,
    R32G32B32_SINT = 0xD5380,

    // 32+32+32+32 bpp
    R32G32B32A32_TYPELESS = 0xE0470,
    R32G32B32A32_UINT = 0xE4470,
    R32G32B32A32_SINT = 0xE5470,
    R32G32B32A32_FLOAT = 0xE6470,

    // 32+8+24bpp
    R32G8X24_TYPELESS = 0xF0360,
    X32_TYPELESS_G8X24_UINT = 0xF4160,
    R32_FLOAT_X8X24_TYPELESS = 0xF6160,
    D32_FLOAT_S8X24_UINT = 0xF9260,

    // bc1
    BC1_UNORM = 0x107420,
    BC1_UNORM_SRGB = 0x108420,

    // bc2
    BC2_UNORM = 0x117430,
    BC2_UNORM_SRGB = 0x118430,

    // bc3
    BC3_UNORM = 0x127430,
    BC3_UNORM_SRGB = 0x128430,

    // bc4
    BC4_UNORM = 0x137120,
    BC4_SNORM = 0x137121,

    // bc5
    BC5_UNORM = 0x147230,
    BC5_SNORM = 0x147231,

    // bc6
    BC6H_UF16 = 0x157330,
    BC6H_SF16 = 0x157331,

    // bc7
    BC7_UNORM = 0x167430,
    BC7_UNORM_SRGB = 0x168430,
};