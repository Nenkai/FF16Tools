using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class BattleCondition : TimelineElementBase, ISerializableStruct
{
    public BattleCondition()
    {
        UnionType = TimelineElementType.BattleCondition;
    }

    public byte field_0x00;
    public byte field_0x01;
    public byte field_0x02;
    public byte field_0x03;
    public byte field_0x04;
    public byte field_0x05;
    public byte field_0x06;
    public byte field_0x07;
    public byte field_0x08;
    public byte field_0x09;
    public byte field_0x0A;
    public byte field_0x0B;
    public float field_0x0C;
    public int field_0x10;
    public float field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;

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
        field_0x08 = bs.Read1Byte();
        field_0x09 = bs.Read1Byte();
        field_0x0A = bs.Read1Byte();
        field_0x0B = bs.Read1Byte();
        field_0x0C = bs.ReadSingle();
        field_0x10 = bs.ReadInt32();
        field_0x14 = bs.ReadSingle();
        field_0x18 = bs.ReadInt32();
        field_0x1C = bs.ReadInt32();
        field_0x20 = bs.ReadInt32();
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
        bs.WriteByte(field_0x08);
        bs.WriteByte(field_0x09);
        bs.WriteByte(field_0x0A);
        bs.WriteByte(field_0x0B);
        bs.WriteSingle(field_0x0C);
        bs.WriteInt32(field_0x10);
        bs.WriteSingle(field_0x14);
        bs.WriteInt32(field_0x18);
        bs.WriteInt32(field_0x1C);
        bs.WriteInt32(field_0x20);
    }

    public uint GetSize() => 0x24;
}

