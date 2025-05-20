using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MdlFlexVertexInfo : ISerializableStruct
{
    public ushort Idx; //Attribute idx
    public ushort Count; //Attribute count

    public void Read(SmartBinaryStream bs)
    {
        Idx = bs.ReadUInt16();
        Count = bs.ReadUInt16();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt16(Idx);
        bs.WriteUInt16(Count);
    }

    public uint GetSize() => 0x04;
}
