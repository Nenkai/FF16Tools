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

    public int Field_0x00 { get; set; }
    public int Field_0x04 { get; set; }
    public float Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }
    public int Field_0x10 { get; set; }
    public float Field_0x14 { get; set; }
    public int Field_0x18 { get; set; }
    public float Field_0x1C { get; set; }
    public int Field_0x20 { get; set; }
    public int Field_0x24 { get; set; }
    public float Field_0x28 { get; set; }
    public int Field_0x2C { get; set; }
    public int Field_0x30 { get; set; }
    public float Field_0x34 { get; set; }
    public int Field_0x38 { get; set; }
    public int Field_0x3C { get; set; }
    public int Field_0x40 { get; set; }
    public int Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }
    public int Field_0x50 { get; set; }
    public int Field_0x54 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.ReadInt32();
        Field_0x04 = bs.ReadInt32();
        Field_0x08 = bs.ReadSingle();
        Field_0x0C = bs.ReadInt32();
        Field_0x10 = bs.ReadInt32();
        Field_0x14 = bs.ReadSingle();
        Field_0x18 = bs.ReadInt32();
        Field_0x1C = bs.ReadSingle();
        Field_0x20 = bs.ReadInt32();
        Field_0x24 = bs.ReadInt32();
        Field_0x28 = bs.ReadSingle();
        Field_0x2C = bs.ReadInt32();
        Field_0x30 = bs.ReadInt32();
        Field_0x34 = bs.ReadSingle();
        Field_0x38 = bs.ReadInt32();
        Field_0x3C = bs.ReadInt32();
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadInt32();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        Field_0x50 = bs.ReadInt32();
        Field_0x54 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(Field_0x00);
        bs.WriteInt32(Field_0x04);
        bs.WriteSingle(Field_0x08);
        bs.WriteInt32(Field_0x0C);
        bs.WriteInt32(Field_0x10);
        bs.WriteSingle(Field_0x14);
        bs.WriteInt32(Field_0x18);
        bs.WriteSingle(Field_0x1C);
        bs.WriteInt32(Field_0x20);
        bs.WriteInt32(Field_0x24);
        bs.WriteSingle(Field_0x28);
        bs.WriteInt32(Field_0x2C);
        bs.WriteInt32(Field_0x30);
        bs.WriteSingle(Field_0x34);
        bs.WriteInt32(Field_0x38);
        bs.WriteInt32(Field_0x3C);
        bs.WriteInt32(Field_0x40);
        bs.WriteInt32(Field_0x44);
        bs.WriteInt32(Field_0x48);
        bs.WriteInt32(Field_0x4C);
        bs.WriteInt32(Field_0x50);
        bs.WriteInt32(Field_0x54);
    }

    public uint GetSize() => GetMetaSize() + 0x58;
}

