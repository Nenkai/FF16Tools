using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files;

public interface ISerializableStruct
{
    public void Read(SmartBinaryStream bs);
    public void Write(SmartBinaryStream bs);
    public uint GetSize();
}
