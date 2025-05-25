using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1123 : TimelineElementBase, ITimelineRangeElement
{
    public TimelineElement_1123()
    {
        UnionType = TimelineElementType.kTimelineElem_1123;
    }

    public int Field_0x00 { get; set; }
    public int Field_0x04 { get; set; }
    public int Field_0x08 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.ReadInt32();
        Field_0x04 = bs.ReadInt32();
        Field_0x08 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(Field_0x00);
        bs.WriteInt32(Field_0x04);
        bs.WriteInt32(Field_0x08);
    }

    public override uint GetSize() => GetMetaSize() + 0x0C;
}

