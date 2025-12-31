using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.UI;

internal class UITimelineElement5050 : TimelineElementBase, ITimelineRangeElement
{
    public override uint GetSize() => 0x68;

    public int Field_0x30 { get; set; }
    public int Field_0x50 { get; set; }
    public float Field_0x54 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);
        bs.ReadCheckPadding(0x20);

        Field_0x30 = bs.ReadInt32();
        bs.ReadCheckPadding(0x1C);
        Field_0x50 = bs.ReadInt32();
        Field_0x54 = bs.ReadSingle();
        bs.ReadCheckPadding(0x20);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);
        bs.WritePadding(0x20);

        bs.WriteInt32(Field_0x30);
        bs.WritePadding(0x1C);
        bs.WriteInt32(Field_0x50);
        bs.WriteSingle(Field_0x54);
        bs.WritePadding(0x20);
    }
}
