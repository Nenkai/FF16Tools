using FF16Tools.Files.UI.Nodes.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public class GUINinegridNode : GUINodeBase<GUINinegridNodeData>
{
    public GUINinegridNode()
    {
        NodeType = GUINodeType.NinegridNode;
    }

    public override uint GetSize()
    {
        return base.GetSize() + 0x2C + 0x20;
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
