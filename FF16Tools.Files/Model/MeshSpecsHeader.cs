using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MeshSpecsHeader : ISerializableStruct
{
    public uint ModelExternalContentSize;
    public ushort SubmeshCount;
    public ushort OptionCount;
    public ushort DrawPartCount;
    public ushort MaterialCount;
    public uint JointCount;

    public ushort Unknown1;
    public ushort LODModelCount;

    public byte Unknown3a;
    public byte FaceJointCount;

    public ushort MuscleJointCount;

    public byte UnkJointParamCount;
    public byte AdditionalPartCount;
    public byte Unknown6;
    public byte VFXEntryCount;

    public uint ExtraSectionSize;
    public uint FlexVertexCount;
    public uint StringTableSize; //string pool size at the end 
    public uint FormatFlags;
    public uint Unknown10;

    public float Unknown11;
    public float Unknown12;

    public uint UnknownBuffer1DecompressedSize;
    public uint UnknownBuffer2DecompressedSize;

    public void Read(SmartBinaryStream bs)
    {
        ModelExternalContentSize = bs.ReadUInt32();
        SubmeshCount = bs.ReadUInt16();
        OptionCount = bs.ReadUInt16();
        DrawPartCount = bs.ReadUInt16();
        MaterialCount = bs.ReadUInt16();
        JointCount = bs.ReadUInt32();

        Unknown1 = bs.ReadUInt16();
        LODModelCount = bs.ReadUInt16();

        Unknown3a = bs.Read1Byte();
        FaceJointCount = bs.Read1Byte();

        MuscleJointCount = bs.ReadUInt16();

        UnkJointParamCount = bs.Read1Byte();
        AdditionalPartCount = bs.Read1Byte();
        Unknown6 = bs.Read1Byte();
        VFXEntryCount = bs.Read1Byte();

        ExtraSectionSize = bs.ReadUInt32();
        FlexVertexCount = bs.ReadUInt32();
        StringTableSize = bs.ReadUInt32();
        FormatFlags = bs.ReadUInt32();
        Unknown10 = bs.ReadUInt32();

        Unknown11 = bs.ReadSingle();
        Unknown12 = bs.ReadSingle();

        UnknownBuffer1DecompressedSize = bs.ReadUInt32();
        UnknownBuffer2DecompressedSize = bs.ReadUInt32();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(ModelExternalContentSize);
        bs.WriteUInt16(SubmeshCount);
        bs.WriteUInt16(OptionCount);
        bs.WriteUInt16(DrawPartCount);
        bs.WriteUInt16(MaterialCount);
        bs.WriteUInt32(JointCount);

        bs.WriteUInt16(Unknown1);
        bs.WriteUInt16(LODModelCount);

        bs.WriteByte(Unknown3a);
        bs.WriteByte(FaceJointCount);

        bs.WriteUInt16(MuscleJointCount);

        bs.WriteByte(UnkJointParamCount);
        bs.WriteByte(AdditionalPartCount);
        bs.WriteByte(Unknown6);
        bs.WriteByte(VFXEntryCount);

        bs.WriteUInt32(ExtraSectionSize);
        bs.WriteUInt32(FlexVertexCount);
        bs.WriteUInt32(StringTableSize);
        bs.WriteUInt32(FormatFlags);
        bs.WriteUInt32(Unknown10);

        bs.WriteSingle(Unknown11);
        bs.WriteSingle(Unknown12);

        bs.WriteUInt32(UnknownBuffer1DecompressedSize);
        bs.WriteUInt32(UnknownBuffer2DecompressedSize);
    }

    public uint GetSize() => 0x28;
}
