using FF16Tools.Files.UI.Nodes.Data;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public abstract class GUINodeBase<TData> : GUINodeBase
    where TData : GUINodeDataBase, new()
{
    public TData Data { get; set; } = new TData();

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        NodeType = (GUINodeType)bs.ReadUInt32();
        Name = bs.ReadStringPointer(basePos);
        Origin = bs.ReadStructMarshal<Point>();
        Rotation = bs.ReadSingle();
        Scale = bs.ReadStructMarshal<Vector2>();
        AnchorPoint = bs.ReadStructMarshal<Point>();
        IsAnchored = bs.ReadBoolean(BooleanCoding.Dword);
        bs.ReadCheckPadding(0x1C);
        Size = bs.ReadStructMarshal<Point>();
        Data = bs.ReadStructPointer<TData>(basePos);
        Field_0x50 = bs.ReadInt32();
        bs.ReadCheckPadding(0x1C);
    }

    public override void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        long lastDataOffset = bs.Position + GetSize();

        bs.WriteUInt32((uint)NodeType);
        bs.AddStringPointer(Name, basePos);
        bs.WriteStructMarshal(Origin);
        bs.WriteSingle(Rotation);
        bs.WriteStructMarshal(Scale);
        bs.WriteStructMarshal(AnchorPoint);
        bs.WriteBoolean(IsAnchored, BooleanCoding.Dword);
        bs.WritePadding(0x1C);
        bs.WriteStructMarshal(Size);
        bs.WriteStructPointer(basePos, Data, ref lastDataOffset);
        bs.WriteInt32(Field_0x50);
        bs.WritePadding(0x1C);
        WriteExtraData(bs, basePos, ref lastDataOffset);

        bs.Position = lastDataOffset;
    }
}