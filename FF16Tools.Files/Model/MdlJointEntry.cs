using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class JointEntry : ISerializableStruct
{
    public uint NameOffset { get; set; }
    public Vector3 WorldPosition { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        NameOffset = bs.ReadUInt32();
        WorldPosition = bs.ReadStructMarshal<Vector3>();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(NameOffset);
        bs.WriteStructMarshal(WorldPosition);
    }

    public uint GetSize() => 0x10;
}
