using FF16Tools.Files.UI.Assets;
using FF16Tools.Files.UI.Nodes.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public class GUIReferenceNode : GUINodeBase<GUIReferenceNodeData>
{
    public string ReferenceName { get; set; }
    public UIAssetReference ReferenceAsset { get; set; }

    public GUIReferenceNode()
    {
        NodeType = GUINodeType.ReferenceNode;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        base.Read(bs);

        ReferenceName = bs.ReadStringPointer(basePos);
        ReferenceAsset = bs.ReadStructPointer<UIAssetReference>(basePos);
    }

    public override void ReadData(SmartBinaryStream bs)
    {
        Data.Read(bs);
    }
}
