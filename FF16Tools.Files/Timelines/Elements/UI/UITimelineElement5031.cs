using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.UI;

internal class UITimelineElement5031 : TimelineElementBase, ITimelineRangeElement
{
    public override uint GetSize() => 0x60;

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);
        bs.ReadCheckPadding(0x20);

        bs.ReadCheckPadding(0x40);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);
        bs.WritePadding(0x20);

        bs.WritePadding(0x40);
    }
}
