using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1016 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1016()
    {
        UnionType = TimelineElementType.PrecedeInputUnk;
    }

    public byte Field_0x00 { get; set; }
    public byte Field_0x01 { get; set; }
    public byte Field_0x02 { get; set; }
    public byte Field_0x03 { get; set; }
    public byte Field_0x04 { get; set; }
    public byte Field_0x05 { get; set; }
    public byte Field_0x06 { get; set; }
    public byte Field_0x07 { get; set; }
    public int Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }
    public int Field_0x10 { get; set; }
    public int Field_0x14 { get; set; }
    public int Field_0x18 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.Read1Byte();
        Field_0x01 = bs.Read1Byte();
        Field_0x02 = bs.Read1Byte();
        Field_0x03 = bs.Read1Byte();
        Field_0x04 = bs.Read1Byte();
        Field_0x05 = bs.Read1Byte();
        Field_0x06 = bs.Read1Byte();
        Field_0x07 = bs.Read1Byte();
        Field_0x08 = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
        Field_0x10 = bs.ReadInt32();
        Field_0x14 = bs.ReadInt32();
        Field_0x18 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteByte(Field_0x00);
        bs.WriteByte(Field_0x01);
        bs.WriteByte(Field_0x02);
        bs.WriteByte(Field_0x03);
        bs.WriteByte(Field_0x04);
        bs.WriteByte(Field_0x05);
        bs.WriteByte(Field_0x06);
        bs.WriteByte(Field_0x07);
        bs.WriteInt32(Field_0x08);
        bs.WriteInt32(Field_0x0C);
        bs.WriteInt32(Field_0x10);
        bs.WriteInt32(Field_0x14);
        bs.WriteInt32(Field_0x18);
    }

    public uint GetSize() => GetMetaSize() + 0x1C;
}

