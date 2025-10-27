using FF16Tools.Files.UI.Nodes.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public class GUITextNode : GUINodeBase<GUITextNodeData>
{
    public int Field_0x70 { get; set; }

    public GUITextNode()
    {
        NodeType = GUINodeType.TextNode;
    }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        Field_0x70 = bs.ReadInt32();
        bs.ReadCheckPadding(0x1C);
        bs.ReadCheckPadding(0x20);
    }

    public override void ReadData(SmartBinaryStream bs)
    {
        Data.Read(bs);
    }
}
