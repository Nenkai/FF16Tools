using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1117 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1117()
    {
        UnionType = TimelineElementType.kTimelineElem_1117;
    }

    /// <summary>
    /// Some kind of Id (1-88), for example 1/2 spawn the satelite skill, 11 starts skill cooldown
    /// </summary>
    public int Field_0x00 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.ReadInt32();
        bs.Position += 0x20; // Padding
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(Field_0x00);
        bs.WritePadding(0x20);
    }

    public uint GetSize() => GetMetaSize() + 0x24;
}

