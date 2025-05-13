using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class SummonPartsVisibleRange : TimelineElementBase, ISerializableStruct
{
    public SummonPartsVisibleRange()
    {
        UnionType = TimelineElementType.SummonPartsVisibleRange;
    }

    public int SummonPartsPatternId;
    public float field_0x04;
    public float field_0x08;
    public byte field_0x0C;
    public byte field_0x0D;
    public byte field_0x0E;
    public byte field_0x0F;
    public int field_0x10;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        SummonPartsPatternId = bs.ReadInt32();
        field_0x04 = bs.ReadSingle();
        field_0x08 = bs.ReadSingle();
        field_0x0C = bs.Read1Byte();
        field_0x0D = bs.Read1Byte();
        field_0x0E = bs.Read1Byte();
        field_0x0F = bs.Read1Byte();
        field_0x10 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(SummonPartsPatternId);
        bs.WriteSingle(field_0x04);
        bs.WriteSingle(field_0x08);
        bs.WriteByte(field_0x0C);
        bs.WriteByte(field_0x0D);
        bs.WriteByte(field_0x0E);
        bs.WriteByte(field_0x0F);
        bs.WriteInt32(field_0x10);
    }

    public uint GetSize() => GetMetaSize() + 0x14;
}

