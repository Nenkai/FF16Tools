using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Components;

public class UIComponentBahamutEffect : UIComponentPropertiesBase
{
    public float Field_0x108 { get; set; }
    public float Field_0x10C { get; set; }
    public float Field_0x110 { get; set; }
    public float Field_0x114 { get; set; }
    public float Field_0x118 { get; set; }
    public float Field_0x11C { get; set; }
    public int Field_0x120 { get; set; }
    public int Field_0x124 { get; set; }
    public int Field_0x128 { get; set; }
    public float Field_0x12C { get; set; }
    public float Field_0x130 { get; set; }
    public float Field_0x134 { get; set; }
    public float Field_0x138 { get; set; }
    public float Field_0x13C { get; set; }
    public float Field_0x140 { get; set; }
    public float Field_0x144 { get; set; }
    public float Field_0x148 { get; set; }
    public float Field_0x14C { get; set; }
    public float Field_0x150 { get; set; }
    public int Field_0x154 { get; set; }
    public int Field_0x158 { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        base.Read(bs);
        Field_0x108 = bs.ReadSingle();
        Field_0x10C = bs.ReadSingle();
        Field_0x110 = bs.ReadSingle();
        Field_0x114 = bs.ReadSingle();
        Field_0x118 = bs.ReadSingle();
        Field_0x11C = bs.ReadSingle();
        Field_0x120 = bs.ReadInt32();
        Field_0x124 = bs.ReadInt32();
        Field_0x128 = bs.ReadInt32();
        Field_0x12C = bs.ReadSingle();
        Field_0x130 = bs.ReadSingle();
        Field_0x134 = bs.ReadSingle();
        Field_0x138 = bs.ReadSingle();
        Field_0x13C = bs.ReadSingle();
        Field_0x140 = bs.ReadSingle();
        Field_0x144 = bs.ReadSingle();
        Field_0x148 = bs.ReadSingle();
        Field_0x14C = bs.ReadSingle();
        Field_0x150 = bs.ReadSingle();
        Field_0x154 = bs.ReadInt32();
        Field_0x158 = bs.ReadInt32();
        bs.ReadCheckPadding(0x20);
    }
}
