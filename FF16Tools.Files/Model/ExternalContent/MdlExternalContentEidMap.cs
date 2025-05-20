using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model.ExternalContent;

public class MdlExternalContentEidMap : MdlExternalContentTypeBase
{
    public uint Field_0x04 { get; set; }
    public uint Field_0x08 { get; set; }
    public uint Field_0x0C { get; set; }
    public List<EidDataDef> EidDataDefinitions { get; set; } = [];

    public MdlExternalContentEidMap()
    {
        Type = MdlExternalContentType.EidMap;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        Type = (MdlExternalContentType)bs.ReadUInt32();
        Field_0x04 = bs.ReadUInt32();
        Field_0x08 = bs.ReadUInt32();
        Field_0x0C = bs.ReadUInt32();
        uint entriesOffset = bs.ReadUInt32();
        uint entryCount = bs.ReadUInt32();

        long endPos = bs.Position;

        for (int i = 0; i < entryCount; i++)
        {
            bs.Position = basePos + entriesOffset + i * 0x2C;

            var dataDef = new EidDataDef();
            dataDef.Read(bs);
            EidDataDefinitions.Add(dataDef);
        }
        bs.Position = endPos;
    }

    public class EidDataDef
    {
        public uint Field_0x00 { get; set; }
        public uint Field_0x04 { get; set; }
        public string? Name { get; set; }
        public float Field_0x0C { get; set; }
        public float Field_0x10 { get; set; }
        public float Field_0x14 { get; set; }
        public float Field_0x18 { get; set; }
        public float Field_0x1C { get; set; }
        public float Field_0x20 { get; set; }
        public uint EidId_Definition { get; set; }
        public uint Field_0x28 { get; set; }

        public void Read(SmartBinaryStream bs)
        {
            long basePos = bs.Position;

            Field_0x00 = bs.ReadUInt32();
            Field_0x04 = bs.ReadUInt32();
            Name = bs.ReadStringPointer(basePos);
            Field_0x0C = bs.ReadSingle();
            Field_0x10 = bs.ReadSingle();
            Field_0x14 = bs.ReadSingle();
            Field_0x18 = bs.ReadSingle();
            Field_0x1C = bs.ReadSingle();
            Field_0x20 = bs.ReadSingle();
            EidId_Definition = bs.ReadUInt32();
            Field_0x28 = bs.ReadUInt32();
        }
    }
}
