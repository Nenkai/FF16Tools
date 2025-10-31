using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Components;

public class UIComponentRoot : UIComponentPropertiesBase
{
    public uint Field_0x108 { get; set; }
    public uint Field_0x10C { get; set; }

    public override uint GetSize()
    {
        return base.GetSize() + 0x20;
    }

    public override void ReadExtraData(SmartBinaryStream bs)
    {
        Field_0x108 = bs.ReadUInt32();
        Field_0x10C = bs.ReadUInt32();
        bs.ReadCheckPadding(0x18);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteUInt32(Field_0x108);
        bs.WriteUInt32(Field_0x10C);
        bs.WritePadding(0x18);
    }
}
