using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic;

public class MagicEntry : ISerializableStruct
{
    public uint Id { get; set; }
    public MagicOperationGroupList OperationGroupList { get; set; }
    public uint OperationGroupsDataSize { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        Id = bs.ReadUInt32();
        OperationGroupList = bs.ReadStructPointer<MagicOperationGroupList>(basePos);
        OperationGroupsDataSize = bs.ReadUInt32();
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        long lastDataPos = bs.GetMarker().LastDataPosition;
        long oldLastDataPos = lastDataPos;

        bs.WriteUInt32(Id);
        bs.WriteStructPointer(basePos, OperationGroupList, ref lastDataPos);
        uint operationsDataSize = (uint)(lastDataPos - oldLastDataPos);
        bs.WriteUInt32(operationsDataSize);

        bs.GetMarker().LastDataPosition = lastDataPos;
    }

    public uint GetSize() => 0x0C;
}
