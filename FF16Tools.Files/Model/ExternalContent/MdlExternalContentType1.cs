using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model.ExternalContent;

public class MdlExternalContentType1 : MdlExternalContentTypeBase
{
    public MdlExternalContentType1()
    {
        Type = MdlExternalContentType.Unk1;
    }

    public uint Field_0x04 { get; set; }
    public uint Field_0x08 { get; set; }
    public uint Field_0x0C { get; set; }
    public uint Field_0x18 { get; set; }
    public uint Field_0x1C { get; set; }
    public uint Field_0x20 { get; set; }
    public uint Field_0x24 { get; set; }
    // Has to be anim (1009)
    public AssetReference UnkAnimAssetRef { get; private set; } = new();
    public uint Field_0x30 { get; set; }
    public float Field_0x34 { get; set; }
    public uint Field_0x38 { get; set; }
    public uint Field_0x3C { get; set; }
    public uint Field_0x40 { get; set; }
    public AssetReference UnkSkeletonAssetRef { get; private set; } = new();
    public byte Field_0x4C { get; set; }
    public byte Field_0x4D { get; set; }
    public byte Field_0x4E { get; set; }
    public byte Field_0x4F { get; set; }
    public uint Field_0x50 { get; set; }
    public uint Field_0x54 { get; set; }
    public uint Field_0x58 { get; set; }
    public uint Field_0x5C { get; set; }
    public uint Field_0x60 { get; set; }
    public byte[] UnkHavokFileHeader { get; set; } // Used by env/bgparts/a/f00/model/ba_f00_acce_bed01.mdl
    public byte[] UnkHavokFileHeader2 { get; set; } // Used by env/bgparts/a/f00/model/ba_f00_buil_base01a.mdl
    public byte[] UnkHavokFileHeader3 { get; set; } // Used by env/bgparts/a/f00/model/ba_f00_buil_gatedoor02.mdl
    public byte[] UnkBonamikBinaryHeader { get; set; } // Used by env/bgparts/a/s01/model/ba_s01_acce_cloth04.mdl

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        Type = (MdlExternalContentType)bs.ReadUInt32();
        Field_0x04 = bs.ReadUInt32();
        Field_0x08 = bs.ReadUInt32();
        Field_0x0C = bs.ReadUInt32();
        int offset_0x10 = bs.ReadInt32(); // TODO
        int count_0x14 = bs.ReadInt32();
        Field_0x18 = bs.ReadUInt32();
        Field_0x1C = bs.ReadUInt32();
        Field_0x20 = bs.ReadUInt32();
        Field_0x24 = bs.ReadUInt32();
        UnkAnimAssetRef.Read(bs);
        Field_0x30 = bs.ReadUInt32();
        Field_0x34 = bs.ReadUInt32();
        Field_0x38 = bs.ReadUInt32();
        Field_0x3C = bs.ReadUInt32();
        Field_0x40 = bs.ReadUInt32();
        UnkSkeletonAssetRef.Read(bs);
        Field_0x4C = bs.Read1Byte();
        Field_0x4D = bs.Read1Byte();
        Field_0x4E = bs.Read1Byte();
        Field_0x4F = bs.Read1Byte();
        Field_0x50 = bs.ReadUInt32();
        Field_0x54 = bs.ReadUInt32();
        Field_0x58 = bs.ReadUInt32();
        Field_0x5C = bs.ReadUInt32();
        Field_0x60 = bs.ReadUInt32();
        int unkHavokFileHeaderOffset = bs.ReadInt32();
        int unkHavokFileHeaderSize = bs.ReadInt32();
        int unkHavokFileHeaderOffset2 = bs.ReadInt32();
        int unkHavokFileHeaderSize2 = bs.ReadInt32();
        int unkHavokFileHeaderOffset3 = bs.ReadInt32();
        int unkHavokFileHeaderSize3 = bs.ReadInt32();
        int unkBonamikBinaryHeaderOffset = bs.ReadInt32();
        int unkBonamikBinaryHeaderSize = bs.ReadInt32();
        int offset_0x84 = bs.ReadInt32(); // TODO
        int offset_0x88 = bs.ReadInt32(); // TODO
        long endPos = bs.Position;

        bs.Position = basePos + unkHavokFileHeaderOffset;
        UnkHavokFileHeader = bs.ReadBytes(unkHavokFileHeaderSize);

        bs.Position = basePos + unkHavokFileHeaderOffset2;
        UnkHavokFileHeader2 = bs.ReadBytes(unkHavokFileHeaderSize2);

        bs.Position = basePos + unkHavokFileHeaderOffset3;
        UnkHavokFileHeader3 = bs.ReadBytes(unkHavokFileHeaderSize3);

        bs.Position = basePos + unkBonamikBinaryHeaderOffset;
        UnkBonamikBinaryHeader = bs.ReadBytes(unkBonamikBinaryHeaderSize);

        bs.Position = endPos;
    }
}
