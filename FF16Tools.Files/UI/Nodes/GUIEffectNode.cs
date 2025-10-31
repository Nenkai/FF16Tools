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

    public override uint GetSize()
    {
        return base.GetSize() + 0x04 + 0x04 + 0x1C;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        bs.ReadCheckPadding(0x04);
        EffectAsset = bs.ReadStructPointer<UIAssetReference>(basePos);
        bs.ReadCheckPadding(0x1C);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WritePadding(0x04);
        bs.WriteStructPointer(basePos, EffectAsset, ref lastDataOffset);
        bs.WritePadding(0x1C);
    }
}
