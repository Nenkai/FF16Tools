using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIMaskNodeData : GUINodeDataBase
{
    public List<UITextureAssetReference> TextureAssetReferences { get; set; } = [];

    public override uint GetSize()
    {
        return base.GetSize() + 0x08 + 0x40;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        TextureAssetReferences = bs.ReadStructArrayFromOffsetCount<UITextureAssetReference>(basePos);
        bs.ReadCheckPadding(0x40);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteStructArrayPointer(basePos, TextureAssetReferences, ref lastDataOffset);
        bs.WritePadding(0x40);
    }
}
