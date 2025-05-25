using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_60 : TimelineElementBase, ITimelineRangeElement
{
    public TimelineElement_60()
    {
        UnionType = TimelineElementType.kTimelineElem_60;
    }

    public int Field_0x00 { get; set; }
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
    public int Field_0x38 { get; set; }
    public int Field_0x3C { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        ReadMeta(bs);

        Field_0x00 = bs.ReadInt32();
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
        Field_0x38 = bs.ReadInt32();
        Field_0x3C = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        WriteMeta(bs);

        bs.WriteInt32(Field_0x00);
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
        bs.WriteInt32(Field_0x38);
        bs.WriteInt32(Field_0x3C);

    }

    public override uint GetSize() => GetMetaSize() + 0x40;
}

