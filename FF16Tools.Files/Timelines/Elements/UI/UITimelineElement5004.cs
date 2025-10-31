using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.UI;

internal class UITimelineElement5004 : TimelineElementBase, ITimelineRangeElement
{
    public override uint GetSize() => 0x6C;

    public int Field_0x30 { get; set; }
    public float Field_0x74 { get; set; }
    public float Field_0x78 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);
        bs.ReadCheckPadding(0x20);

        Field_0x30 = bs.ReadInt32();
        bs.ReadCheckPadding(0x40);
        Field_0x74 = bs.ReadSingle();
        Field_0x78 = bs.ReadSingle();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);
        bs.WritePadding(0x20);

        bs.WriteInt32(Field_0x30);
        bs.WritePadding(0x40);
        bs.WriteSingle(Field_0x74);
        bs.WriteSingle(Field_0x78);
    }
}
