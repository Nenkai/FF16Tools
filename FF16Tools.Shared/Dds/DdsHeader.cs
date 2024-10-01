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

        bs.WriteInt32(0x1000 | 0x400000 | 0x08); // dwCaps, 0x1000 = required
        bs.WriteBytes(new byte[16]); // dwCaps2-4 + reserved

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
