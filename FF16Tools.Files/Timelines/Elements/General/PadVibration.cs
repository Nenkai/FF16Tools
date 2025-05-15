using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class PadVibration : TimelineElementBase, ISerializableStruct
{
    public PadVibration()
    {
        UnionType = TimelineElementType.PadVibration;
    }

    public int CameraFCurveId { get; set; }
    public string? UnkName1 { get; set; }
    public int Field_0x08 { get; set; }
    public string? UnkName2 { get; set; }
    public int Field_0x10 { get; set; }
    public string? UnkName3 { get; set; }
    public int Field_0x18 { get; set; }
    public int Field_0x1C { get; set; }
    public int Field_0x20 { get; set; }
    public int Field_0x24 { get; set; }
    public int Field_0x28 { get; set; }
    public int Field_0x2C { get; set; }
    public int Field_0x30 { get; set; }
    public int Field_0x34 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        ReadMeta(bs);

        CameraFCurveId = bs.ReadInt32();
        UnkName1 = bs.ReadStringPointer(baseMetaPos);
        Field_0x08 = bs.ReadInt32();
        UnkName2 = bs.ReadStringPointer(baseMetaPos);
        Field_0x10 = bs.ReadInt32();
        UnkName3 = bs.ReadStringPointer(baseMetaPos);
        Field_0x18 = bs.ReadInt32();
        Field_0x1C = bs.ReadInt32();
        Field_0x20 = bs.ReadInt32();
        Field_0x24 = bs.ReadInt32();
        Field_0x28 = bs.ReadInt32();
        Field_0x2C = bs.ReadInt32();
        Field_0x30 = bs.ReadInt32();
        Field_0x34 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        WriteMeta(bs);

        bs.WriteInt32(CameraFCurveId);
        bs.AddStringPointer(UnkName1, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(Field_0x08);
        bs.AddStringPointer(UnkName2, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(Field_0x10);
        bs.AddStringPointer(UnkName3, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(Field_0x18);
        bs.WriteInt32(Field_0x1C);
        bs.WriteInt32(Field_0x20);
        bs.WriteInt32(Field_0x24);
        bs.WriteInt32(Field_0x28);
        bs.WriteInt32(Field_0x2C);
        bs.WriteInt32(Field_0x30);
        bs.WriteInt32(Field_0x34);
    }

    public uint GetSize() => GetMetaSize() + 0x38;
}

