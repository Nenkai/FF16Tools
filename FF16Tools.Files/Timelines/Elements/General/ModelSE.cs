using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class ModelSE : TimelineElementBase, ITimelineTriggerElement
{
    public ModelSE()
    {
        UnionType = TimelineElementType.ModelSE;
    }

    public int Field_0x00 { get; set; }
    public bool Bool_0x04 { get; set; }
    public int EidId { get; set; }
    public int Field_0x0C { get; set; }
    public double Field_0x10 { get; set; }
    public double Field_0x18 { get; set; }
    public double Field_0x20 { get; set; }
    public int Field_0x28 { get; set; }
    public float Field_0x2C { get; set; }
    public float Field_0x30 { get; set; }
    public float Field_0x34 { get; set; }
    public int Field_0x38 { get; set; }
    public float Field_0x3C { get; set; }
    public int Field_0x40 { get; set; }
    public int Field_0x44 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Field_0x00 = bs.ReadInt32();
        Bool_0x04 = bs.ReadBoolean();
        bs.ReadCheckPadding(0x03);
        EidId = bs.ReadInt32();
        Field_0x0C = bs.ReadInt32();
        Field_0x10 = bs.ReadDouble();
        Field_0x18 = bs.ReadDouble();
        Field_0x20 = bs.ReadDouble();
        Field_0x28 = bs.ReadInt32();
        Field_0x2C = bs.ReadSingle();
        Field_0x30 = bs.ReadSingle();
        Field_0x34 = bs.ReadSingle();
        Field_0x38 = bs.ReadInt32();
        Field_0x3C = bs.ReadSingle();
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(Field_0x00);
        bs.WriteBoolean(Bool_0x04);
        bs.WritePadding(0x03);
        bs.WriteInt32(EidId);
        bs.WriteInt32(Field_0x0C);
        bs.WriteDouble(Field_0x10);
        bs.WriteDouble(Field_0x18);
        bs.WriteDouble(Field_0x20);
        bs.WriteInt32(Field_0x28);
        bs.WriteSingle(Field_0x2C);
        bs.WriteSingle(Field_0x30);
        bs.WriteSingle(Field_0x34);
        bs.WriteInt32(Field_0x38);
        bs.WriteSingle(Field_0x3C);
        bs.WriteInt32(Field_0x40);
        bs.WriteInt32(Field_0x44);
    }

    public override uint GetSize() => GetMetaSize() + 0x48;
}

