using FF16Tools.Files.UI.Nodes.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public class GUILayerNode : GUINodeBase<GUILayerNodeData>
{
    public List<GUINodeBase> Nodes { get; set; } = [];
    public GUILayerNode()
    {
        NodeType = GUINodeType.LayerNode;
    }

    public override uint GetSize()
    {
        return base.GetSize() + 0x20 + 0x04 + 0x04;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        bs.ReadCheckPadding(0x20);

        int childNodesOffset = bs.ReadInt32();
        int childNodeCount = bs.ReadInt32();

        for (int i = 0; i < childNodeCount; i++)
        {
            bs.Position = basePos + childNodesOffset + (i * sizeof(int));
            int nodeOffset = bs.ReadInt32();

            bs.Position = basePos + childNodesOffset + nodeOffset;
            GUINodeType nodeType = (GUINodeType)bs.ReadInt32();
            GUINodeBase nodeBase = GUINodeBase.Create(nodeType);
            bs.Position -= 4;

            nodeBase.Read(bs);
            Nodes.Add(nodeBase);
        }
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WritePadding(0x20);
        bs.WriteStructArrayPointerWithOffsetTable32(basePos, Nodes, ref lastDataOffset);
    }
}
