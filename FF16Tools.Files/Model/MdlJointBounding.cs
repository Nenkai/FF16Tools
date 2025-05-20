using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MdlJointBounding : ISerializableStruct
{
    public Vector3 BoundingMin;
    public Vector3 BoundingMax;
    public float Unknown1;
    public float Unknown2;

    public void Read(SmartBinaryStream bs)
    {
        BoundingMin = bs.ReadVector3();
        BoundingMax = bs.ReadVector3();
        Unknown1 = bs.ReadSingle();
        Unknown2 = bs.ReadSingle();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteVector3(BoundingMin);
        bs.WriteVector3(BoundingMax);
        bs.WriteSingle(Unknown1);
        bs.WriteSingle(Unknown2);
    }

    public uint GetSize() => 0x20;
}
