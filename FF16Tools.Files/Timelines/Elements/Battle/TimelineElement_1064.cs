using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1064 : TimelineElementBase, ITimelineRangeElement
{
    public TimelineElement_1064()
    {
        UnionType = TimelineElementType.kTimelineElem_1064;
    }

    public int AttackParamId { get; set; }
    public string? Name { get; set; }
    public int Field_0x08 { get; set; }
    public int Field_0x0C { get; set; }
    public int Field_0x10 { get; set; }
    public int Field_0x14 { get; set; }
    public int Field_0x18 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        ReadMeta(bs);

        AttackParamId = bs.ReadInt32();
        Name = bs.ReadStringPointer(basePos);
        Field_0x08 = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
        Field_0x10 = bs.ReadInt32();
        Field_0x14 = bs.ReadInt32();
        Field_0x18 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        WriteMeta(bs);

        bs.WriteInt32(AttackParamId);
        bs.AddStringPointer(Name, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(Field_0x08);
        bs.WriteInt32(Field_0x0C);
        bs.WriteInt32(Field_0x10);
        bs.WriteInt32(Field_0x14);
        bs.WriteInt32(Field_0x18);
    }

    public override uint GetSize() => GetMetaSize() + 0x1C;
}

