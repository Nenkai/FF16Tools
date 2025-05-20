using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MdlFlexVertexAttribute : ISerializableStruct
{
    public byte BufferIdx;
    public byte Offset;
    public EncodingFormat Format;
    public MdlVertexSemantic Type;

    public MdlFlexVertexAttribute() { }

    public MdlFlexVertexAttribute(byte buffer, byte offset, MdlVertexSemantic type, EncodingFormat format)
    {
        BufferIdx = buffer;
        Offset = offset;
        Format = format;
        Type = type;
    }

    public void Read(SmartBinaryStream bs)
    {
        BufferIdx = bs.Read1Byte();
        Offset = bs.Read1Byte();
        Type = (MdlVertexSemantic)bs.ReadByte();
        Format = (EncodingFormat)bs.ReadByte();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.Write(BufferIdx);
        bs.Write(Offset);
        bs.Write((byte)Type);
        bs.Write((byte)Format);
    }

    public uint GetSize() => 0x04;

    /// <summary>
    /// Size of one vertex.
    /// </summary>
    public int ElementSize
    {
        get
        {
            switch (Format)
            {
                case EncodingFormat.UINT8x4:
                case EncodingFormat.UNORM8x4:
                case EncodingFormat.HALFFLOATx2:
                    return 4;
                case EncodingFormat.FLOATx4:
                    return 16;
                case EncodingFormat.FLOATx3:
                    return 12;
                case EncodingFormat.HALFFLOATx4:
                case EncodingFormat.FLOATx2:
                    return 8;
                default:
                    throw new Exception($"{Format} not supported!");
            }
        }
    }

    public override string ToString() => $"{Type}_{Format}_{Offset}_{BufferIdx}";
}

public enum EncodingFormat : byte
{
    SNORM16x2 = 18,   // = 37 = DXGI_FORMAT_R16G16_SNORM
    SNORM16x4 = 20,   // = 13 = DXGI_FORMAT_R16G16B16A16_SNORM
    FLOAT = 33,       // = 41 = DXGI_FORMAT_R32_FLOAT
    FLOATx2 = 34,     // = 16 = DXGI_FORMAT_R32G32_FLOAT
    FLOATx3 = 35,     // = 6  = DXGI_FORMAT_R32G32B32A32_FLOAT
    FLOATx4 = 36,     // = 2  = DXGI_FORMAT_R32G32B32A32_FLOAT
    HALFFLOATx2 = 50, // = 34 = DXGI_FORMAT_R16G16_FLOAT
    HALFFLOATx4 = 52, // = 10 = DXGI_FORMAT_R16G16B16A16_FLOAT
    UNORM8x4 = 68,    // = 28 = DXGI_FORMAT_R8G8_B8G8_UNORM
    SINT16x2 = 82,    // = 38 = DXGI_FORMAT_R16G16_SINT
    SINT16x4 = 84,    // = 14 = DXGI_FORMAT_R16G16B16A16_SINT
    UINT8x4 = 116,    // = 30 = DXGI_FORMAT_R8G8B8A8_UINT
    UINT16x2 = 130,   // = 36 = DXGI_FORMAT_R16G16_UINT
    UINT16x4 = 132,   // = 12 = DXGI_FORMAT_R16G16B16A16_UINT
    SINT32 = 145,     // = 43 = DXGI_FORMAT_R32_SINT 
    SINT32x2 = 146,   // = 18 = DXGI_FORMAT_R32G32_SINT
    SINT32x3 = 147,   // = 8  = DXGI_FORMAT_R32G32B32_SINT
    SINT32x4 = 148,   // = 4  = DXGI_FORMAT_R32G32B32A32_SINT  
    UINT32 = 161,     // = 42 = DXGI_FORMAT_R32_UINT
    UINT32x2 = 162,   // = 17 = DXGI_FORMAT_R32G32_UINT
    UINT32x3 = 163,   // = 7  = DXGI_FORMAT_R32G32B32_UINT
    UINT32x4 = 164,   // = 3  = DXGI_FORMAT_R32G32B32A32_UINT
    UNORM16x2 = 178,  // = 35 = DXGI_FORMAT_R16G16_UNORM
    UNORM16x4 = 180,  // = 11 = DXGI_FORMAT_R16G16B16A16_UNORM
}

public enum MdlVertexSemantic : byte
{
    POSITION = 0,
    BLENDWEIGHT_0 = 1,
    BLENDINDICES_0 = 2,
    COLOR_0 = 3,
    COLOR_1 = 4,
    COLOR_2 = 5,
    COLOR_3 = 6,
    COLOR_4 = 7,
    COLOR_5 = 8,
    COLOR_6 = 9,
    COLOR_7 = 10,
    TEXCOORD_0 = 11,
    TEXCOORD_1 = 12,
    TEXCOORD_2 = 13,
    TEXCOORD_3 = 14,
    TEXCOORD_4 = 15,
    TEXCOORD_5 = 16,
    TEXCOORD_6 = 17,
    TEXCOORD_7 = 18,
    TEXCOORD_8 = 19,
    TEXCOORD_9 = 20,
    TEXCOORD_10_NORMAL = 21, // Used by normals
    TEXCOORD_11_TANGENT = 22, // Used by tangent
    TEXCOORD_12_BITANGENT = 23, // Used by bitangents
    TEXCOORD_13_UNK = 24,
    TEXCOORD_14_UNK = 25,
    TEXCOORD_15_UNK = 26,
    DEPTH = 27,
    BLENDWEIGHT_1 = 28,
    BLENDINDICES_1 = 29,
}
