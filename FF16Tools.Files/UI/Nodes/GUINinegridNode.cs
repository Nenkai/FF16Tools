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

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        bs.ReadCheckPadding(0x1C);
        bs.ReadCheckPadding(0x20);
    }

    public override void ReadData(SmartBinaryStream bs)
    {
        Data.Read(bs);
    }
}
