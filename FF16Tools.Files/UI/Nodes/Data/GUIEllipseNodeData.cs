using FF16Tools.Files.UI.Assets;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUIEllipseNodeData : GUINodeDataBase
{
    public int Field_0x40 { get; set; }
    public float Field_0x44 { get; set; }
    public int Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }
    public List<UITextureAssetReference> TextureAssetReferences { get; set; } = [];

    public override uint GetSize()
    {
        return base.GetSize() + 0x18;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        base.Read(bs);
        Field_0x40 = bs.ReadInt32();
        Field_0x44 = bs.ReadSingle();
        Field_0x48 = bs.ReadInt32();
        Field_0x4C = bs.ReadInt32();
        TextureAssetReferences = bs.ReadStructArrayFromOffsetCount<UITextureAssetReference>(basePos);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteInt32(Field_0x40);
        bs.WriteSingle(Field_0x44);
        bs.WriteInt32(Field_0x48);
        bs.WriteInt32(Field_0x4C);
        bs.WriteStructArrayPointer(basePos, TextureAssetReferences, ref lastDataOffset);
    }
}
