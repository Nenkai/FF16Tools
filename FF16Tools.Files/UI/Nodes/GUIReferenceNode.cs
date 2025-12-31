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

    public override uint GetSize()
    {
        return base.GetSize() + 0x04 + 0x04 + 0x08 + 0x20;
    }

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
        bs.ReadCheckPadding(0x08);
        bs.ReadCheckPadding(0x20);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.AddStringPointer(ReferenceName, basePos);
        bs.WriteStructPointer(basePos, ReferenceAsset, ref lastDataOffset);
        bs.WritePadding(0x08);
        bs.WritePadding(0x20);
    }
}
