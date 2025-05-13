using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1099 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1099()
    {
        UnionType = TimelineElementType.kTimelineElem_1099;
    }

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public float field_0x14;
    public int field_0x18;
    public float field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        field_0x00 = bs.ReadInt32();
        field_0x04 = bs.ReadInt32();
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.ReadInt32();
        field_0x10 = bs.ReadInt32();
        field_0x14 = bs.ReadSingle();
        field_0x18 = bs.ReadInt32();
        field_0x1C = bs.ReadSingle();
        field_0x20 = bs.ReadInt32();
        field_0x24 = bs.ReadInt32();
        field_0x28 = bs.ReadInt32();
        field_0x2C = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(field_0x00);
        bs.WriteInt32(field_0x04);
        bs.WriteInt32(field_0x08);
        bs.WriteInt32(field_0x0C);
        bs.WriteInt32(field_0x10);
        bs.WriteSingle(field_0x14);
        bs.WriteInt32(field_0x18);
        bs.WriteSingle(field_0x1C);
        bs.WriteInt32(field_0x20);
        bs.WriteInt32(field_0x24);
        bs.WriteInt32(field_0x28);
        bs.WriteInt32(field_0x2C);
    }

    public uint GetSize() => GetMetaSize() + 0x30;
}

