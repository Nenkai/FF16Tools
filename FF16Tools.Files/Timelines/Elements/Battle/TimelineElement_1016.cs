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

    public byte field_0x00;
    public byte field_0x01;
    public byte field_0x02;
    public byte field_0x03;
    public byte field_0x04;
    public byte field_0x05;
    public byte field_0x06;
    public byte field_0x07;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        field_0x00 = bs.Read1Byte();
        field_0x01 = bs.Read1Byte();
        field_0x02 = bs.Read1Byte();
        field_0x03 = bs.Read1Byte();
        field_0x04 = bs.Read1Byte();
        field_0x05 = bs.Read1Byte();
        field_0x06 = bs.Read1Byte();
        field_0x07 = bs.Read1Byte();
        field_0x08 = bs.ReadInt32();
        field_0x0C = bs.ReadInt32();
        field_0x10 = bs.ReadInt32();
        field_0x14 = bs.ReadInt32();
        field_0x18 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteByte(field_0x00);
        bs.WriteByte(field_0x01);
        bs.WriteByte(field_0x02);
        bs.WriteByte(field_0x03);
        bs.WriteByte(field_0x04);
        bs.WriteByte(field_0x05);
        bs.WriteByte(field_0x06);
        bs.WriteByte(field_0x07);
        bs.WriteInt32(field_0x08);
        bs.WriteInt32(field_0x0C);
        bs.WriteInt32(field_0x10);
        bs.WriteInt32(field_0x14);
        bs.WriteInt32(field_0x18);
    }

    public uint GetSize() => GetMetaSize() + 0x1C;
}

