using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class BulletTimeRange : TimelineElementBase, ISerializableStruct
{
    public BulletTimeRange()
    {
        UnionType = TimelineElementType.BattleMessageRange;
    }

    public float field_0x00;
    public float field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        field_0x00 = bs.ReadSingle();
        field_0x04 = bs.ReadSingle();
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.ReadInt32();
        field_0x10 = bs.ReadInt32();
        field_0x14 = bs.ReadInt32();
        field_0x18 = bs.ReadInt32();
        field_0x1C = bs.ReadInt32();
        field_0x20 = bs.ReadInt32();
        field_0x24 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteSingle(field_0x00);
        bs.WriteSingle(field_0x04);
        bs.WriteInt32(field_0x08);
        bs.WriteInt32(field_0x0C);
        bs.WriteInt32(field_0x10);
        bs.WriteInt32(field_0x14);
        bs.WriteInt32(field_0x18);
        bs.WriteInt32(field_0x1C);
        bs.WriteInt32(field_0x20);
        bs.WriteInt32(field_0x24);
    }

    public uint GetSize() => 0x28;
}

