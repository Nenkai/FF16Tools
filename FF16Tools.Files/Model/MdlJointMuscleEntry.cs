using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class MdlJointMuscleEntry : ISerializableStruct
{
    public uint NameOffset { get; set; }
    public float Unknown1 { get; set; }
    public ushort[] IndicesSet1 { get; private set; } = new ushort[4];
    public ushort[] IndicesSet2 { get; private set; } = new ushort[4];
    public float[] WeightsSet1 { get; private set; } = new float[4];
    public float[] WeightsSet2 { get; private set; } = new float[4];

    public Vector3 Unknown2 { get; set; }
    public Vector3 Unknown3 { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        NameOffset = bs.ReadUInt32();
        Unknown1 = bs.ReadSingle();
        for (int i = 0; i < 4; i++)
            IndicesSet1[i] = bs.ReadUInt16();
        for (int i = 0; i < 4; i++)
            IndicesSet2[i] = bs.ReadUInt16();
        for (int i = 0; i < 4; i++)
            WeightsSet1[i] = bs.ReadSingle();
        for (int i = 0; i < 4; i++)
            WeightsSet2[i] = bs.ReadSingle();

        Unknown2 = bs.ReadVector3();
        Unknown3 = bs.ReadVector3();
    }

    public void Write(SmartBinaryStream bs)
    {
        bs.WriteUInt32(NameOffset);
        bs.WriteSingle(Unknown1);

        for (int i = 0; i < 4; i++)
            bs.WriteSingle(IndicesSet1[i]);
        for (int i = 0; i < 4; i++)
            bs.WriteSingle(IndicesSet2[i]);
        for (int i = 0; i < 4; i++)
            bs.WriteSingle(WeightsSet1[i]);
        for (int i = 0; i < 4; i++)
            bs.WriteSingle(WeightsSet2[i]);

        bs.WriteVector3(Unknown2);
        bs.WriteVector3(Unknown2);
    }

    public uint GetSize() => 0x50;
}
