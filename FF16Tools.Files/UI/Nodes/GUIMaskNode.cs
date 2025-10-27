using FF16Tools.Files.UI.Assets;
using FF16Tools.Files.UI.Nodes.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public class GUIMaskNode : GUINodeBase<GUIMaskNodeData>
{
    public UIAssetReference MaskAsset { get; set; }
    public string MaskNameUnk { get; set; }

    public GUIMaskNode()
    {
        NodeType = GUINodeType.MaskNode;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        bs.ReadCheckPadding(0x2C);
        MaskAsset = bs.ReadStructPointer<UIAssetReference>(basePos);
        MaskNameUnk = bs.ReadStringPointer(basePos);
        bs.ReadCheckPadding(0x18);
    }

    public override void ReadData(SmartBinaryStream bs)
    {
        Data.Read(bs);
    }
}
