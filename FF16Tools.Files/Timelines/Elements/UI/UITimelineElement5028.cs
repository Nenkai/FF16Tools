using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.UI;

internal class UITimelineElement5028 : TimelineElementBase, ITimelineRangeElement
{
    public override uint GetSize() => 0x64;

    public string Field_0x30 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        ReadMeta(bs);
        bs.ReadCheckPadding(0x20);

        Field_0x30 = bs.ReadStringPointer(basePos);
        bs.ReadCheckPadding(0x40);
    }

    public override void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        WriteMeta(bs);
        bs.WritePadding(0x20);

        bs.AddStringPointer(Field_0x30, basePos);
        bs.WritePadding(0x40);

    }
}
