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

    public int SummonPartsPatternId { get; set; }
    public float Field_0x04 { get; set; }
    public float Field_0x08 { get; set; }
    public byte Field_0x0C { get; set; }
    public byte Field_0x0D { get; set; }
    public byte Field_0x0E { get; set; }
    public byte Field_0x0F { get; set; }
    public int Field_0x10 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        SummonPartsPatternId = bs.ReadInt32();
        Field_0x04 = bs.ReadSingle();
        Field_0x08 = bs.ReadSingle();
        Field_0x0C = bs.Read1Byte();
        Field_0x0D = bs.Read1Byte();
        Field_0x0E = bs.Read1Byte();
        Field_0x0F = bs.Read1Byte();
        Field_0x10 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(SummonPartsPatternId);
        bs.WriteSingle(Field_0x04);
        bs.WriteSingle(Field_0x08);
        bs.WriteByte(Field_0x0C);
        bs.WriteByte(Field_0x0D);
        bs.WriteByte(Field_0x0E);
        bs.WriteByte(Field_0x0F);
        bs.WriteInt32(Field_0x10);
    }

    public uint GetSize() => GetMetaSize() + 0x14;
}

