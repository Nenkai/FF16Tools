using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Components;

public class UIComponentScrollBar : UIComponentPropertiesBase
{
    public int Field_0x108 { get; set; }
    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        Field_0x108 = bs.ReadInt32();
        bs.ReadCheckPadding(0x1C);
    }
}
