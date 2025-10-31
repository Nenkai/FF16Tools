using FF16Tools.Files.UI.Nodes.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public class GUICounterNode : GUINodeBase<GUICounterNodeData>
{
    public GUICounterNode()
    {
        NodeType = GUINodeType.CounterNode;
    }

    public override uint GetSize()
    {
        return base.GetSize() + 0x20 + 0x2C;
    }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        bs.ReadCheckPadding(0x2C);
        bs.ReadCheckPadding(0x20);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WritePadding(0x2C);
        bs.WritePadding(0x20);
    }
}
