using FF16Tools.Files.UI.Assets;
using FF16Tools.Files.UI.Nodes.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public class GUIEffectNode : GUINodeBase<GUIEffectNodeData>
{
    public UIAssetReference EffectAsset { get; set; } = new();

    public GUIEffectNode()
    {
        NodeType = GUINodeType.EffectNode;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        bs.ReadCheckPadding(0x04);
        EffectAsset = bs.ReadStructPointer<UIAssetReference>(basePos);
        bs.ReadCheckPadding(0x1C);
    }

    public override void ReadData(SmartBinaryStream bs)
    {
        Data.Read(bs);
    }
}
