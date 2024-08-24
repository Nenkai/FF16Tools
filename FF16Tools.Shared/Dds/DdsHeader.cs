using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace FF16Tools.Shared.Dds;

public class DdsHeader
{
    public DDSHeaderFlags Flags { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int PitchOrLinearSize { get; set; }
    public int LastMipmapLevel { get; set; }

    public DDSPixelFormatFlags FormatFlags { get; set; }
    public string FourCCName { get; set; }

    public int RGBBitCount { get; set; }
    public uint RBitMask { get; set; }
    public uint GBitMask { get; set; }
    public uint BBitMask { get; set; }
    public uint ABitMask { get; set; }

    public DXGI_FORMAT DxgiFormat { get; set; }

    public Memory<byte> ImageData { get; set; }

    public void Write(Stream outStream)
    {
        var bs = new BinaryStream(outStream);

        bs.WriteString("DDS ", StringCoding.Raw);
        bs.WriteInt32(124);    // dwSize (Struct Size)
        bs.WriteUInt32((uint)Flags); // dwFlags
        bs.WriteInt32(Height); // dwHeight
        bs.WriteInt32(Width);
        bs.WriteInt32(PitchOrLinearSize);
        bs.WriteInt32(0);    // Depth
        bs.WriteInt32(LastMipmapLevel);
        bs.WriteBytes(new byte[44]); // reserved
        bs.WriteInt32(32); // DDSPixelFormat Header starts here - Struct Size


        bs.WriteUInt32((uint)FormatFlags);           // Format Flags
        bs.WriteString(FourCCName, StringCoding.Raw); // FourCC
        bs.WriteInt32(RGBBitCount);         // RGBBitCount

        bs.WriteUInt32(RBitMask);  // RBitMask 
        bs.WriteUInt32(GBitMask);  // GBitMask
        bs.WriteUInt32(BBitMask);  // BBitMask
        bs.WriteUInt32(ABitMask);  // ABitMask

        bs.WriteInt32(0x1000); // dwCaps, 0x1000 = required
        bs.WriteBytes(new byte[16]); // dwCaps1-4

        if (FourCCName == "DX10")
        {
            // DDS_HEADER_DXT10
            bs.WriteInt32((int)DxgiFormat);
            bs.WriteInt32(3);  // DDS_DIMENSION_TEXTURE2D
            bs.BaseStream.Seek(4, SeekOrigin.Current);  // miscFlag
            bs.WriteInt32(1); // arraySize
            bs.WriteInt32(0); // miscFlags2
        }
        bs.Write(ImageData.Span);
    }

    public static int BitsPerPixel(DXGI_FORMAT fmt)
    {
        switch (fmt)
        {
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_SINT:
                return 128;

            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_SINT:
                return 96;

            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G32_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32G8X24_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_X32_TYPELESS_G8X24_UINT:
                return 64;

            case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R11G11B10_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16G16_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R32_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R24G8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R24_UNORM_X8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_X24_TYPELESS_G8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R9G9B9E5_SHAREDEXP:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_B8G8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_G8R8_G8B8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB:
                return 32;

            case DXGI_FORMAT.DXGI_FORMAT_R8G8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R16_FLOAT:
            case DXGI_FORMAT.DXGI_FORMAT_D16_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R16_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R16_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM:
                return 16;

            case DXGI_FORMAT.DXGI_FORMAT_R8_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_R8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8_UINT:
            case DXGI_FORMAT.DXGI_FORMAT_R8_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8_SINT:
            case DXGI_FORMAT.DXGI_FORMAT_A8_UNORM:
                return 8;

            case DXGI_FORMAT.DXGI_FORMAT_R1_UNORM:
                return 1;

            case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                return 4;

            case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                return 8;

            default:
                return 0;
        }
    }
}

[Flags]
public enum DDSPixelFormatFlags
{
    DDPF_ALPHAPIXELS = 0x01,
    DDPF_ALPHA = 0x02,
    DDPF_FOURCC = 0x04,
    DDPF_RGB = 0x40,
    DDPF_YUV = 0x200,
    DDPF_LUMINANCE = 0x20000
}

[Flags]
public enum DDSHeaderFlags : uint
{
    TEXTURE = 0x00001007,  // DDSDCAPS | DDSDHEIGHT | DDSDWIDTH | DDSDPIXELFORMAT 
    MIPMAP = 0x00020000,  // DDSDMIPMAPCOUNT
    VOLUME = 0x00800000,  // DDSDDEPTH
    PITCH = 0x00000008,  // DDSDPITCH
    LINEARSIZE = 0x00080000,  // DDSDLINEARSIZE
}
