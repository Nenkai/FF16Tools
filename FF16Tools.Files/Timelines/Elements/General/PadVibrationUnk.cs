using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class PadVibrationUnk : TimelineElementBase, ISerializableStruct
{
    public PadVibrationUnk()
    {
        UnionType = TimelineElementType.PadVibrationUnk;
    }

    public int CameraFCurveId;
    public string? UnkName1;
    public int field_0x08;
    public string? UnkName2;
    public int field_0x10;
    public string? UnkName3;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;

    public override void Read(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        ReadMeta(bs);

        CameraFCurveId = bs.ReadInt32();
        UnkName1 = bs.ReadStringPointer(baseMetaPos);
        field_0x08 = bs.ReadInt32();
        UnkName2 = bs.ReadStringPointer(baseMetaPos);
        field_0x10 = bs.ReadInt32();
        UnkName3 = bs.ReadStringPointer(baseMetaPos);
        field_0x18 = bs.ReadInt32();
        field_0x1C = bs.ReadInt32();
        field_0x20 = bs.ReadInt32();
        field_0x24 = bs.ReadInt32();
        field_0x28 = bs.ReadInt32();
        field_0x2C = bs.ReadInt32();
        field_0x30 = bs.ReadInt32();
        field_0x34 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        WriteMeta(bs);

        bs.WriteInt32(CameraFCurveId);
        bs.AddStringPointer(UnkName1, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(field_0x08);
        bs.AddStringPointer(UnkName2, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(field_0x10);
        bs.AddStringPointer(UnkName3, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(field_0x18);
        bs.WriteInt32(field_0x1C);
        bs.WriteInt32(field_0x20);
        bs.WriteInt32(field_0x24);
        bs.WriteInt32(field_0x28);
        bs.WriteInt32(field_0x2C);
        bs.WriteInt32(field_0x30);
        bs.WriteInt32(field_0x34);
    }

    public uint GetSize() => GetMetaSize() + 0x38;
}

