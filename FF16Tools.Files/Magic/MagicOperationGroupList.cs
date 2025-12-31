using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic;

public class MagicOperationGroupList : ISerializableStruct
{
    public float Field_0x00 { get; set; }
    public uint Field_0x04 { get; set; }
    public List<MagicOperationGroup> OperationGroups { get; set; } = [];

    public void Read(SmartBinaryStream bs)
    {
        Field_0x00 = bs.ReadSingle();
        Field_0x04 = bs.ReadUInt32();
        uint numOperationGroups = bs.ReadUInt32();
        OperationGroups = bs.ReadStructArray<MagicOperationGroup>(numOperationGroups);
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteSingle(Field_0x00);
        bs.WriteUInt32(Field_0x04);

        bs.WriteUInt32((uint)OperationGroups.Count);
        long lastDataOffset = bs.Position + (new MagicOperationGroupList().GetSize() * OperationGroups.Count);
        foreach (var ent in OperationGroups)
        {
            using var marker = bs.PushMarker(lastDataOffset);
            ent.Write(bs);

            lastDataOffset = marker.LastDataPosition;
        }

        bs.Position = lastDataOffset;
    }

    public uint GetSize() => 0x0C;
}
