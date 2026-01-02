using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic;

public class MagicOperationProperty : ISerializableStruct
{
    public MagicPropertyType Type { get; set; }
    public byte[] Data { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        Type = (MagicPropertyType)bs.ReadUInt32();
        uint dataSize = bs.ReadUInt32();
        Data = bs.ReadBytes((int)dataSize);
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32((uint)Type);
        bs.WriteUInt32((uint)Data.Length);
        bs.WriteBytes(Data);
    }

    public uint GetSize()
    {
        throw new NotImplementedException();
    }
}
