using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MdlFaceJointEntry : ISerializableStruct
{
    public uint Offset;
    public uint Padding;

    public void Read(SmartBinaryStream bs)
    {
        Offset = bs.ReadUInt32();
        Padding = bs.ReadUInt32();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(Offset);
        bs.WriteUInt32(Padding);
    }

    public uint GetSize() => 0x08;
}
