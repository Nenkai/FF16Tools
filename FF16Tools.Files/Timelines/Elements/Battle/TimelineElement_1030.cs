using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static FF16Tools.Files.Timelines.Elements.Battle.TimelineElement_1023;

namespace FF16Tools.Files.Timelines.Elements.Battle;

public class TimelineElement_1030 : TimelineElementBase, ISerializableStruct
{
    public TimelineElement_1030()
    {
        UnionType = TimelineElementType.kTimelineElem_1030;
    }

    public List<Sub1023Struct> SubStructs { get; set; } = [];
    public List<Sub1023Struct2> SubStructs2 { get; set; } = [];
    public AssetReference VFXAssetRef = new();
    public AssetReference UnkAsset2 = new();
    public int Field_0x20 { get; set; }
    public byte Field_0x24 { get; set; }
    public byte Field_0x25 { get; set; }
    public byte Field_0x26 { get; set; }
    public byte Field_0x27 { get; set; }
    public int Field_0x28 { get; set; }
    public int Field_0x2C { get; set; }
    public int Field_0x30 { get; set; }
    public string? UnkName { get; set; }
    public int Field_0x38 { get; set; }
    public int Field_0x3C { get; set; }
    public int Field_0x40 { get; set; }
    public int Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }
    public int Field_0x50 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        long baseOffset = bs.Position;
        ReadMeta(bs);

        long baseDataOffset = bs.Position;

        // May be reverse offset
        int structsOffset = bs.ReadInt32();
        int structsCount = bs.ReadInt32();
        int structs2Offset = bs.ReadInt32();
        int structs2Count = bs.ReadInt32();

        VFXAssetRef.Read(bs);
        UnkAsset2.Read(bs);
        Field_0x20 = bs.ReadInt32();
        Field_0x24 = bs.Read1Byte();
        Field_0x25 = bs.Read1Byte();
        Field_0x26 = bs.Read1Byte();
        Field_0x27 = bs.Read1Byte();
        Field_0x28 = bs.ReadInt32();
        Field_0x2C = bs.ReadInt32();
        Field_0x30 = bs.ReadInt32();
        UnkName = bs.ReadStringPointer(baseOffset);
        Field_0x38 = bs.ReadInt32();
        Field_0x3C = bs.ReadInt32();
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadInt32();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        Field_0x50 = bs.ReadInt32();

        if (structsOffset != 0)
        {
            bs.Position = baseDataOffset + structsOffset;
            for (int i = 0; i < structsCount; i++)
            {
                TimelineElement_1023.Sub1023Struct subStruct = new TimelineElement_1023.Sub1023Struct();
                subStruct.Read(bs);
                SubStructs.Add(subStruct);
            }
        }

        if (structs2Offset != 0)
        {
            bs.Position = baseDataOffset + structs2Offset;
            for (int i = 0; i < structs2Count; i++)
            {
                Sub1023Struct2 subStruct2 = new Sub1023Struct2();
                subStruct2.Read(bs);
                SubStructs2.Add(subStruct2);
            }
        }
    }

    public override void Write(SmartBinaryStream bs)
    {
        long baseOffset = bs.Position;
        WriteMeta(bs);

        long baseDataOffset = bs.Position;

        // This was written previously
        bs.WriteObjectPointer(SubStructs, (int)baseDataOffset, writeZeroIfNullOrNotFound: true); // Will be reverse
        bs.WriteInt32(SubStructs.Count);
        bs.WriteInt32(0x54);
        bs.WriteInt32(SubStructs2.Count);

        VFXAssetRef.Write(bs);
        UnkAsset2.Write(bs);
        bs.WriteInt32(Field_0x20);
        bs.WriteByte(Field_0x24);
        bs.WriteByte(Field_0x25);
        bs.WriteByte(Field_0x26);
        bs.WriteByte(Field_0x27);
        bs.WriteInt32(Field_0x28);
        bs.WriteInt32(Field_0x2C);
        bs.WriteInt32(Field_0x30);
        bs.AddStringPointer(UnkName, baseOffset);
        bs.WriteInt32(Field_0x38);
        bs.WriteInt32(Field_0x3C);
        bs.WriteInt32(Field_0x40);
        bs.WriteInt32(Field_0x44);
        bs.WriteInt32(Field_0x48);
        bs.WriteInt32(Field_0x4C);
        bs.WriteInt32(Field_0x50);

        for (int i = 0; i < SubStructs2.Count; i++)
        {
            SubStructs2[i].Write(bs);
        }
    }

    public uint GetSize() => GetMetaSize() + 0x54 + ((uint)SubStructs2.Count * 0x1C);

}

