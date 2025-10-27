using FF16Tools.Files.UI.Assets;
using FF16Tools.Files.UI.Nodes.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public class GUIRectNode : GUINodeBase<GUIRectNodeData>
{
    public UIAssetReference MaskAsset { get; set; }
    public string MaskNameUnk { get; set; }

    public GUIRectNode()
    {
        NodeType = GUINodeType.RectNode;
    }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        bs.ReadCheckPadding(0x20);
        bs.ReadCheckPadding(0x20);
    }

    public override void ReadData(SmartBinaryStream bs)
    {
        Data.Read(bs);
    }
}
