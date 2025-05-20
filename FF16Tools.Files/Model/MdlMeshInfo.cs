using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MdlMeshInfo : ISerializableStruct
{
    public uint FaceIndexCount { get; set; }
    public uint FaceIndicesOffset { get; set; }
    public ushort VertexCount { get; set; }
    public ushort MaterialID { get; set; }
    public ushort DrawPartStartIndex { get; set; }
    public ushort DrawPartCount { get; set; } //sub draw call that ranges 0 -> 3. Not always used, but reduces draw usage after first level
    public ushort FlexVertexInfoID { get; set; }
    public MdlMeshBoneSetFlags BoneSetFlag { get; set; }
    public MdlMeshTexCoordFlags TexCoordSetFlag { get; set; }
    public uint Flag2 { get; set; }
    public uint[] Unknowns2 { get; private set; } = new uint[6];
    public uint Unknown3 { get; set; }
    public byte Unknown4 { get; set; }
    public byte Unknown5 { get; set; }
    public byte Unknown6 { get; set; }
    public byte UsedBufferCount { get; set; } = 2;
    public uint[] BufferOffsets { get; private set; } = new uint[8];
    public byte[] Strides { get; private set; } = new byte[8];

    public void Read(SmartBinaryStream bs)
    {
        FaceIndexCount = bs.ReadUInt32();
        FaceIndicesOffset = bs.ReadUInt32();
        VertexCount = bs.ReadUInt16();
        MaterialID = bs.ReadUInt16();
        DrawPartStartIndex = bs.ReadUInt16();
        DrawPartCount = bs.ReadUInt16();
        FlexVertexInfoID = bs.ReadUInt16();
        BoneSetFlag = (MdlMeshBoneSetFlags)bs.Read1Byte();
        TexCoordSetFlag = (MdlMeshTexCoordFlags)bs.Read1Byte();
        Flag2 = bs.ReadUInt32();

        for (int i = 0; i < 6; i++)
            Unknowns2[i] = bs.ReadUInt32();

        Unknown3 = bs.ReadUInt32();
        Unknown4 = bs.Read1Byte();
        Unknown5 = bs.Read1Byte();
        Unknown6 = bs.Read1Byte();
        UsedBufferCount = bs.Read1Byte();

        for (int i = 0; i < 8; i++)
            BufferOffsets[i] = bs.ReadUInt32();

        for (int i = 0; i < 8; i++)
            Strides[i] = bs.Read1Byte();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(FaceIndexCount);
        bs.WriteUInt32(FaceIndicesOffset);
        bs.WriteUInt16(VertexCount);
        bs.WriteUInt16(MaterialID);
        bs.WriteUInt16(DrawPartStartIndex);
        bs.WriteUInt16(DrawPartCount);
        bs.WriteUInt16(FlexVertexInfoID);
        bs.WriteByte((byte)BoneSetFlag);
        bs.WriteByte((byte)TexCoordSetFlag);
        bs.WriteUInt32(Flag2);

        for (int i = 0; i < 6; i++)
            bs.WriteUInt32(Unknowns2[i]);

        bs.WriteUInt32(Unknown3);
        bs.WriteByte(Unknown4);
        bs.WriteByte(Unknown5);
        bs.WriteByte(Unknown6);
        bs.WriteByte(UsedBufferCount);

        for (int i = 0; i < 8; i++)
            bs.WriteUInt32(BufferOffsets[i]);

        for (int i = 0; i < 8; i++)
            bs.WriteByte(Strides[i]);
    }

    public uint GetSize() => 0x60;
}

[Flags]
public enum MdlMeshTexCoordFlags : byte
{
    USE_UV0 = 1 << 0,
    USE_UV1 = 1 << 1,
    USE_UV2 = 1 << 2,
    USE_UV3 = 1 << 3,
}

[Flags]
public enum MdlMeshBoneSetFlags : byte
{
    USE_BONESET1 = 1 << 0,
    USE_BONESET0 = 1 << 1,
}