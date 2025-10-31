using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes.Data;

public class GUICounterNodeData : GUINodeDataBase
{
    public List<UITextureAssetReference> TextureAssets { get; set; } = [];
    public string Field_0x68 { get; set; }
    public string Field_0x6C { get; set; }
    public int Field_0x70 { get; set; }
    public int Field_0x74 { get; set; }
    public int Field_0x78 { get; set; }
    public int Field_0x7C { get; set; }
    public int Field_0x80 { get; set; }
    public int Field_0x84 { get; set; }
    public int Field_0x88 { get; set; }
    public int Field_0x8C { get; set; }

    public override uint GetSize()
    {
        return base.GetSize() + 0x4C;
    }

    public override void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        base.Read(bs);
        TextureAssets = bs.ReadStructArrayFromOffsetCount<UITextureAssetReference>(basePos);
        bs.ReadCheckPadding(0x20);

        Field_0x68 = bs.ReadStringPointer(basePos);
        Field_0x6C = bs.ReadStringPointer(basePos);
        Field_0x70 = bs.ReadInt32();
        Field_0x74 = bs.ReadInt32();
        Field_0x78 = bs.ReadInt32();
        Field_0x7C = bs.ReadInt32();
        Field_0x80 = bs.ReadInt32();
        bs.ReadCheckPadding(0x08);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteStructArrayPointer(basePos, TextureAssets, ref lastDataOffset);
        bs.WritePadding(0x20);

        bs.AddStringPointer(Field_0x68, basePos);
        bs.AddStringPointer(Field_0x6C, basePos);
        bs.WriteInt32(Field_0x70);
        bs.WriteInt32(Field_0x74);
        bs.WriteInt32(Field_0x78);
        bs.WriteInt32(Field_0x7C);
        bs.WriteInt32(Field_0x80);
        bs.WritePadding(0x08);
    }
}
