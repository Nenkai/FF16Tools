using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Components;

public class UIComponentScrollBar : UIComponentPropertiesBase
{
    public int Field_0x108 { get; set; }

    public override uint GetSize()
    {
        return base.GetSize() + 0x20;
    }

    public override void ReadExtraData(SmartBinaryStream bs)
    {
        Field_0x108 = bs.ReadInt32();
        bs.ReadCheckPadding(0x1C);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WriteInt32(Field_0x108);
        bs.WritePadding(0x1C);
    }
}
