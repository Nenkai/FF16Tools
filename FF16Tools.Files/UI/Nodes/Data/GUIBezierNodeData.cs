using FF16Tools.Files.UI.Assets;

using Syroot.BinaryData;

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

    public override uint GetSize()
    {
        return base.GetSize() + 0x48;
    }

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
        Points = bs.ReadStructArrayFromOffsetCount<BezierPoint>(basePos);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteInt32(Field_0x40);
        bs.WriteInt32(Field_0x44);
        bs.WriteInt32(Field_0x48);
        bs.WriteInt32(Field_0x4C);
        bs.WriteInt32(Field_0x50);
        bs.WriteInt32(Field_0x54);
        bs.WriteSingle(Field_0x58);
        bs.WriteSingle(Field_0x5C);
        bs.WriteSingle(Field_0x60);
        bs.WriteSingle(Field_0x64);
        bs.WriteInt32(Field_0x65);
        bs.WritePadding(1);
        bs.WritePadding(1);
        bs.WriteInt32(Field_0x68);
        bs.WriteSingle(Field_0x6C);
        bs.WriteSingle(Field_0x70);
        bs.WriteSingle(Field_0x74);
        bs.WriteSingle(Field_0x78);
        bs.WriteSingle(Field_0x7C);
        bs.WriteStructArrayPointer(basePos, Points, ref lastDataOffset);
    }
}

public class BezierPoint : ISerializableStruct
{
    public Point Origin { get; set; }
    public Point HandleA { get; set; }
    public Point HandleB { get; set; }
    public float Field_0x18 { get; set; }
    public int Field_0x1C { get; set; }

    public uint GetSize()
    {
        return 0x38;
    }

    public void Read(SmartBinaryStream bs)
    {
        Origin = bs.ReadStructMarshal<Point>();
        HandleA = bs.ReadStructMarshal<Point>();
        HandleB = bs.ReadStructMarshal<Point>();
        Field_0x18 = bs.ReadSingle();
        Field_0x1C = bs.ReadInt32();
        bs.ReadCheckPadding(0x18);
    }

    public void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();
    }
}
