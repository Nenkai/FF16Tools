using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Components;

public class UIComponentTab : UIComponentPropertiesBase
{
    public override uint GetSize()
    {
        return base.GetSize() + 0x20;
    }

    public override void ReadExtraData(SmartBinaryStream bs)
    {
        bs.ReadCheckPadding(0x20);
    }

    public override void WriteExtraData(SmartBinaryStream bs, long basePos, ref long lastDataOffset)
    {
        bs.WritePadding(0x20);
    }
}
