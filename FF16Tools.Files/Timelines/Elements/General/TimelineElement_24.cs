using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_24 : TimelineElementBase, ITimelineRangeElement
{
    public TimelineElement_24()
    {
        UnionType = TimelineElementType.kTimelineElem_24;
    }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);
        bs.ReadCheckPadding(0x20);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WritePadding(0x20);
    }

    public override uint GetSize() => GetMetaSize() + 0x20;
}

