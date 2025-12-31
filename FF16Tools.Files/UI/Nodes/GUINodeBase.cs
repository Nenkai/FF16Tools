using FF16Tools.Files.UI.Nodes.Data;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Vortice.Direct3D12;

namespace FF16Tools.Files.UI.Nodes;

public abstract class GUINodeBase : ISerializableStruct
{
    public GUINodeType NodeType { get; protected set; }
    public string Name { get; set; }
    public Point Origin { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
    public Point AnchorPoint { get; set; }
    public bool IsAnchored { get; set; }
    public Point Size { get; set; }
    public int Field_0x50 { get; set; }

    public virtual uint GetSize()
    {
        return 0x70;
    }

    public abstract void Write(SmartBinaryStream bs);
    public abstract void Read(SmartBinaryStream bs);

    public static GUINodeBase Create(GUINodeType type)
    {
        GUINodeBase nodeBase = type switch
        {
            GUINodeType.LayerNode => new GUILayerNode(),
            GUINodeType.ImageNode => new GUIImageNode(),
            GUINodeType.TextNode => new GUITextNode(),
            GUINodeType.NinegridNode => new GUINinegridNode(),
            GUINodeType.CounterNode => new GUICounterNode(),
            GUINodeType.RectNode => new GUIRectNode(),
            GUINodeType.EffectNode => new GUIEffectNode(),
            GUINodeType.EllipseNode => new GUIEllipseNode(),
            GUINodeType.BezierNode => new GUIBezierNode(),
            GUINodeType.CollisionNode => new GUICollisionNode(),
            GUINodeType.ReferenceNode => new GUIReferenceNode(),
            // ModelNode
            GUINodeType.MaskNode => new GUIMaskNode(),
            _ => throw new NotImplementedException($"Not implemented: {type}")
        };
        return nodeBase;
    }

    public abstract void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset);
}

public enum GUINodeType
{
    LayerNode = 1,
    ImageNode = 2,
    TextNode = 3,
    NinegridNode = 4,
    CounterNode = 5,
    RectNode = 6,
    EllipseNode = 7,
    BezierNode = 8,
    CollisionNode = 9,
    ReferenceNode = 10,
    EffectNode = 11,
    ModelNode = 12,
    MaskNode = 13
};
