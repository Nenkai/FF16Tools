using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class PlaySoundTrigger : TimelineElementBase, ISerializableStruct
{
    public PlaySoundTrigger()
    {
        UnionType = TimelineElementType.PlaySoundTrigger;
    }

    public AssetReference SoundAsset { get; set; } = new();
    public int Field_0x08 { get; set; }
    public byte Field_0x0C { get; set; }
    public int Field_0x10 { get; set; }
    public int Field_0x14 { get; set; }
    public double Field_0x18 { get; set; }
    public double Field_0x20 { get; set; }
    public double Field_0x28 { get; set; }
    public int Field_0x30 { get; set; }
    public int Field_0x34 { get; set; }
    public int Field_0x38 { get; set; }
    public float Field_0x3C { get; set; }
    public byte Field_0x40 { get; set; }
    public float Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        SoundAsset.Read(bs);
        Field_0x08 = bs.ReadInt32();
        Field_0x0C = bs.Read1Byte();
        bs.ReadCheckPadding(0x03);
        Field_0x10 = bs.ReadInt32();
        Field_0x14 = bs.ReadInt32();
        Field_0x18 = bs.ReadDouble();
        Field_0x20 = bs.ReadDouble();
        Field_0x28 = bs.ReadDouble();
        Field_0x30 = bs.ReadInt32();
        Field_0x34 = bs.ReadInt32();
        Field_0x38 = bs.ReadInt32();
        Field_0x3C = bs.ReadSingle();
        Field_0x40 = bs.Read1Byte();
        bs.ReadCheckPadding(0x03);
        Field_0x44 = bs.ReadSingle();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        bs.ReadCheckPadding(0x10);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        SoundAsset.Write(bs);
        bs.WriteInt32(Field_0x08);
        bs.WriteByte(Field_0x0C);
        bs.WritePadding(0x03);
        bs.WriteInt32(Field_0x10);
        bs.WriteInt32(Field_0x14);
        bs.WriteDouble(Field_0x18);
        bs.WriteDouble(Field_0x20);
        bs.WriteDouble(Field_0x28);
        bs.WriteInt32(Field_0x30);
        bs.WriteInt32(Field_0x34);
        bs.WriteInt32(Field_0x38);
        bs.WriteSingle(Field_0x3C);
        bs.WriteByte(Field_0x40);
        bs.WritePadding(0x03);
        bs.WriteSingle(Field_0x44);
        bs.WriteInt32(Field_0x48);
        bs.WriteInt32(Field_0x4C);
        bs.WritePadding(0x10);
    }

    public uint GetSize() => GetMetaSize() + 0x60;
}

