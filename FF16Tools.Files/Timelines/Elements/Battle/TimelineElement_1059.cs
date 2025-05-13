using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1059 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1059()
    {
        UnionType = TimelineElementType.kTimelineElem_1059;
    }

    public string? Name { get; set; }
    public float Field_0x04 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        ReadMeta(bs);

        Name = bs.ReadStringPointer(basePos);
        Field_0x04 = bs.ReadSingle();
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseMetaPos = bs.Position;
        WriteMeta(bs);

        bs.AddStringPointer(Name, relativeBaseOffset: baseMetaPos);
        bs.WriteSingle(Field_0x04);
    }

    public uint GetSize() => GetMetaSize() + 0x08;
}

