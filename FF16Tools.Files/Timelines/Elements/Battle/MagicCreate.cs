using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class MagicCreate : TimelineElementBase, ITimelineTriggerElement
{
    public MagicCreate()
    {
        UnionType = TimelineElementType.MagicCreate;
    }

    public int Unused { get; set; }
    public int MagicId { get; set; }
    public bool Field_0x08 { get; set; }
    public byte Field_0x09 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Unused = bs.ReadInt32();
        MagicId = bs.ReadInt32();
        Field_0x08 = bs.ReadBoolean();
        Field_0x09 = bs.Read1Byte();
        bs.ReadCheckPadding(0x1E);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(Unused);
        bs.WriteInt32(MagicId);
        bs.WriteBoolean(Field_0x08);
        bs.WriteByte(Field_0x09);
        bs.WritePadding(0x1E);
    }

    public override uint GetSize() => GetMetaSize() + 0x28;
}

