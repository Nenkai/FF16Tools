using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Assets;

public class UIAssetReference : ISerializableStruct
{
    public uint Type { get; set; }

    /// <summary>
    /// Must start with UITextureParts:// for Textures <br/>
    /// Must start with UI:// for UI<br/>
    /// </summary>
    public string Path { get; set; }

    public AssetReference AssetReference { get; set; } = new();
    
    public uint GetSize()
    {
        return 0x2C;
    }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        Type = bs.ReadUInt32();
        Path = bs.ReadStringPointer(basePos);
        AssetReference = bs.ReadStructPointer<AssetReference>(basePos);
        bs.ReadCheckPadding(0x20);
    }

    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        long lastDataOffset = bs.Position + GetSize();

        bs.WriteUInt32(Type);
        bs.AddStringPointer(Path, basePos);
        bs.WriteStructPointer(basePos, AssetReference, ref lastDataOffset);
        bs.WritePadding(0x20);

        bs.Position = lastDataOffset;
    }
}
