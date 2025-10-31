using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Assets;

public class UITextureAssetReference : ISerializableStruct
{
    public UIAssetReference AssetReference { get; set; } = new();
    public uint Unk_0x04 { get; set; }
    public uint Unk_0x08 { get; set; }

    public uint GetSize()
    {
        return 0x2C;
    }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        AssetReference = bs.ReadStructPointer<UIAssetReference>(basePos);
        Unk_0x04 = bs.ReadUInt32();
        Unk_0x08 = bs.ReadUInt32();
        bs.ReadCheckPadding(0x20);
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        long lastDataOffset = bs.Position + GetSize();
        bs.WriteStructPointer(basePos, AssetReference, ref lastDataOffset);
        bs.WriteUInt32(Unk_0x04);
        bs.WriteUInt32(Unk_0x08);
        bs.WritePadding(0x20);

        bs.Position = lastDataOffset;
    }
}
