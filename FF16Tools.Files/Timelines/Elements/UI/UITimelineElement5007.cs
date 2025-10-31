using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.UI;

internal class UITimelineElement5007 : TimelineElementBase, ITimelineRangeElement
{
    public override uint GetSize() => 0x68;

    public int Field_0x30 { get; set; }
    public byte Field_0x74 { get; set; }
    public byte Field_0x75 { get; set; }
    public byte Field_0x76 { get; set; }
    public byte Field_0x77 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);
        bs.ReadCheckPadding(0x20);

        Field_0x30 = bs.ReadInt32();
        bs.ReadCheckPadding(0x40);
        Field_0x74 = bs.Read1Byte();
        Field_0x75 = bs.Read1Byte();
        Field_0x76 = bs.Read1Byte();
        Field_0x77 = bs.Read1Byte();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);
        bs.WritePadding(0x20);

        bs.WriteInt32(Field_0x30);
        bs.WritePadding(0x40);
        bs.WriteByte(Field_0x74);
        bs.WriteByte(Field_0x75);
        bs.WriteByte(Field_0x76);
        bs.WriteByte(Field_0x77);

    }
}
