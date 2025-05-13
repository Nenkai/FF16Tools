using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1115 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1115()
    {
        UnionType = TimelineElementType.kTimelineElem_1115;
    }

    public int field_0x00;
    public int field_0x04;
    public float field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public float field_0x14;
    public int field_0x18;
    public float field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public float field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public float field_0x34;
    public int field_0x38;
    public int field_0x3C;
    public int field_0x40;
    public int field_0x44;
    public int field_0x48;
    public int field_0x4C;
    public int field_0x50;
    public int field_0x54;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        field_0x00 = bs.ReadInt32();
        field_0x04 = bs.ReadInt32();
        field_0x08 = bs.ReadSingle();
        field_0x0C = bs.ReadInt32();
        field_0x10 = bs.ReadInt32();
        field_0x14 = bs.ReadSingle();
        field_0x18 = bs.ReadInt32();
        field_0x1C = bs.ReadSingle();
        field_0x20 = bs.ReadInt32();
        field_0x24 = bs.ReadInt32();
        field_0x28 = bs.ReadSingle();
        field_0x2C = bs.ReadInt32();
        field_0x30 = bs.ReadInt32();
        field_0x34 = bs.ReadSingle();
        field_0x38 = bs.ReadInt32();
        field_0x3C = bs.ReadInt32();
        field_0x40 = bs.ReadInt32();
        field_0x44 = bs.ReadInt32();
        field_0x48 = bs.ReadInt32();
        field_0x4C = bs.ReadInt32();
        field_0x50 = bs.ReadInt32();
        field_0x54 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(field_0x00);
        bs.WriteInt32(field_0x04);
        bs.WriteSingle(field_0x08);
        bs.WriteInt32(field_0x0C);
        bs.WriteInt32(field_0x10);
        bs.WriteSingle(field_0x14);
        bs.WriteInt32(field_0x18);
        bs.WriteSingle(field_0x1C);
        bs.WriteInt32(field_0x20);
        bs.WriteInt32(field_0x24);
        bs.WriteSingle(field_0x28);
        bs.WriteInt32(field_0x2C);
        bs.WriteInt32(field_0x30);
        bs.WriteSingle(field_0x34);
        bs.WriteInt32(field_0x38);
        bs.WriteInt32(field_0x3C);
        bs.WriteInt32(field_0x40);
        bs.WriteInt32(field_0x44);
        bs.WriteInt32(field_0x48);
        bs.WriteInt32(field_0x4C);
        bs.WriteInt32(field_0x50);
        bs.WriteInt32(field_0x54);
    }

    public uint GetSize() => GetMetaSize() + 0x58;
}

