using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIImageNodeData : GUINodeDataBase
{
    public int Field_0x68 { get; set; }
    public int Field_0x6C { get; set; }
    public int Field_0x70 { get; set; }
    public int Field_0x74 { get; set; }
    public List<UITextureAssetReference> TextureAssetReferences { get; set; } = [];

    public override uint GetSize()
    {
        return base.GetSize() + 0x54;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        TextureAssetReferences = bs.ReadStructArrayFromOffsetCount<UITextureAssetReference>(basePos);
        bs.ReadCheckPadding(0x20);

        Field_0x68 = bs.ReadInt32();
        Field_0x6C = bs.ReadInt32();
        Field_0x70 = bs.ReadInt32();
        Field_0x74 = bs.ReadInt32();
        bs.ReadCheckPadding(0x1C);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteStructArrayPointer(basePos, TextureAssetReferences, ref lastDataOffset);
        bs.WritePadding(0x20);

        bs.WriteInt32(Field_0x68);
        bs.WriteInt32(Field_0x6C);
        bs.WriteInt32(Field_0x70);
        bs.WriteInt32(Field_0x74);
        bs.WritePadding(0x1C);
    }
}
