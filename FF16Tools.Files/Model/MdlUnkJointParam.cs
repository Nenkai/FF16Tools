using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MdlUnkJointParam : ISerializableStruct
{
    public float[] a { get; private set; } = new float[7];
    public ushort b { get; set; }
    public ushort c { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        for (int i = 0; i < 7; i++)
            a[i] = bs.ReadSingle();
        b = bs.ReadUInt16();
        c = bs.ReadUInt16();
    }

    public void Write(SmartBinaryStream bs)
    {
        for (int i = 0; i < 7; i++)
            bs.WriteSingle(a[i]);

        bs.WriteUInt16(b);
        bs.WriteUInt16(c);
    }

    public uint GetSize() => 0x20;
}
