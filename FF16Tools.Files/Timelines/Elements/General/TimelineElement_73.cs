using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_73 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_73()
    {
        UnionType = TimelineElementType.kTimelineElem_73;
    }

    public int field_0x00;
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
    public int field_0x38;
    public int field_0x3C;

    public override void Read(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        ReadMeta(bs);

        field_0x00 = bs.ReadInt32();
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
        field_0x38 = bs.ReadInt32();
        field_0x3C = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        WriteMeta(bs);

        bs.WriteInt32(field_0x00);
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
        bs.WriteInt32(field_0x38);
        bs.WriteInt32(field_0x3C);

    }

    public uint GetSize() => GetMetaSize() + 0x40;
}
