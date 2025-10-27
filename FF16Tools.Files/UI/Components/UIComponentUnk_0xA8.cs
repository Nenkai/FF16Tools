using FF16Tools.Files.UI.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Components;

public class UIComponentUnk_0xA8 : ISerializableStruct
{
    public UIAssetReference UIAssetRef { get; set; }
    public uint Unk_0x08 { get; set; }

    public uint GetSize()
    {
        return 0x2C;
    }

    public void Read(SmartBinaryStream bs)
    {
        uint uiAssetRefOffset = bs.ReadUInt32();
        uint field_0x04 = bs.ReadUInt32();
        Unk_0x08 = bs.ReadUInt32();
        bs.ReadCheckPadding(0x20);
    }

    public void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();
    }
}
