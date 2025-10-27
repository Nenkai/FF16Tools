using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIBezierNodeData : GUINodeDataBase
{
    public int Field_0x40 { get; set; }
    public int Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }
    public int Field_0x50 { get; set; }
    public int Field_0x54 { get; set; }
    public float Field_0x58 { get; set; }
    public float Field_0x5C { get; set; }
    public float Field_0x60 { get; set; }
    public byte Field_0x64 { get; set; }
    public byte Field_0x65 { get; set; }
    public int Field_0x68 { get; set; }
    public float Field_0x6C { get; set; }
    public float Field_0x70 { get; set; }
    public float Field_0x74 { get; set; }
    public float Field_0x78 { get; set; }
    public float Field_0x7C { get; set; }
    public List<BezierPoint> Points { get; set; } = [];

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadInt32();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        Field_0x50 = bs.ReadInt32();
        Field_0x54 = bs.ReadInt32();
        Field_0x58 = bs.ReadSingle();
        Field_0x5C = bs.ReadSingle();
        Field_0x60 = bs.ReadSingle();
        Field_0x64 = bs.Read1Byte();
        Field_0x65 = bs.Read1Byte();
        bs.ReadCheckPadding(1);
        bs.ReadCheckPadding(1);
        Field_0x68 = bs.ReadInt32();
        Field_0x6C = bs.ReadSingle();
        Field_0x70 = bs.ReadSingle();
        Field_0x74 = bs.ReadSingle();
        Field_0x78 = bs.ReadSingle();
        Field_0x7C = bs.ReadSingle();
        int nodesOffset = bs.ReadInt32();
        uint nodesCount = bs.ReadUInt32();

        bs.Position = basePos + nodesOffset;
        Points = bs.ReadArrayOfStructs<BezierPoint>(nodesCount);
    }
}

public class BezierPoint : ISerializableStruct
{
    public Point Origin { get; set; }
    public Point HandleA { get; set; }
    public Point HandleB { get; set; }
    public float Field_0x18 { get; set; }

    public uint GetSize()
    {
        return 0x38;
    }

    public void Read(SmartBinaryStream bs)
    {
        Origin = bs.ReadPoint();
        HandleA = bs.ReadPoint();
        HandleB = bs.ReadPoint();
        Field_0x18 = bs.ReadSingle();
        bs.ReadCheckPadding(0x18);
    }

    public void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();
    }
}
