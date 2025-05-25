using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1103 : TimelineElementBase, ITimelineRangeElement
{
    public TimelineElement_1103()
    {
        UnionType = TimelineElementType.kTimelineElem_1103;
    }
    public string? Name { get; set; }
    public int Field_0x04 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        ReadMeta(bs);

        Name = bs.ReadStringPointer(basePos);
        Field_0x04 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        WriteMeta(bs);

        bs.AddStringPointer(Name, relativeBaseOffset: baseMetaPos);
        bs.WriteInt32(Field_0x04);
    }

    public override uint GetSize() => GetMetaSize() + 0x08;
}

