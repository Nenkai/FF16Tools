using FF16Tools.Files.UI.Assets;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Components;

public class UIComponentPropertiesBase : ISerializableStruct
{
    public UIComponentType Type { get; set; }
    public List<string> Names { get; set; } = [];
    public uint SubType { get; set; }
    public BlendMode BlendMode { get; set; }
    public Color UnkColor_0x14 { get; set; }
    public uint Unk_0x18 { get; set; }
    public uint UnkPercentage0_0x1C { get; set; }
    public uint UnkPercentage1 { get; set; }
    public uint UnkPercentage2 { get; set; }
    public int Field_0x28 { get; set; }
    public float Field_0x2C { get; set; }
    public float Field_0x30 { get; set; }
    public float Field_0x34 { get; set; }
    public float Field_0x38 { get; set; }
    public float Field_0x3C { get; set; }
    public float Field_0x40 { get; set; }
    public float Field_0x44 { get; set; }
    public float Field_0x48 { get; set; }
    public int Field_0x4C { get; set; }
    public float Field_0x50 { get; set; }
    public float Field_0x54 { get; set; }
    public float Field_0x58 { get; set; }
    public float Field_0x5C { get; set; }
    public short Field_0x60 { get; set; }
    public byte Field_0x62 { get; set; }
    public byte Field_0x63 { get; set; }
    public float Field_0x64 { get; set; }
    public uint Type_0x68 { get; set; }
    public float Field_0x6C { get; set; }
    public float Field_0x70 { get; set; }
    public byte Unk_0x71 { get; set; }
    public byte Unk_0x72 { get; set; }
    public byte Unk_0x73 { get; set; }
    public byte Unk_0x74 { get; set; }
    public float Field_0x78 { get; set; }
    public float Field_0x7C { get; set; }
    public float Field_0x80 { get; set; }
    public float Field_0x84 { get; set; }
    public float Field_0x88 { get; set; }
    public uint Type_0x8C { get; set; }
    public float Field_0x90 { get; set; }
    public float Field_0x94 { get; set; }
    public UIAssetReference UIAssetReference_0x98 { get; set; } = new();
    public float Field_0x9C { get; set; }
    public float Field_0xA0 { get; set; }
    public float Field_0xA4 { get; set; }
    public UIComponentUnk_0xA8 Unk0xA8 { get; set; }
    public uint Field_0xB0 { get; set; }
    public uint Field_0xB4 { get; set; }
    public uint Field_0xB8 { get; set; }
    public float Field_0xBC { get; set; }
    public float Field_0xC0 { get; set; }
    public float Field_0xC4 { get; set; }
    public float Field_0xC8 { get; set; }
    public uint Field_0xCC { get; set; }
    public float Field_0xD0 { get; set; }
    public float Field_0xD4 { get; set; }
    public float Field_0xD8 { get; set; }
    public float Field_0xDC { get; set; }
    public float Field_0xE0 { get; set; }
    public float Field_0xE4 { get; set; }
    public float Field_0xE8 { get; set; }
    public float Field_0xEC { get; set; }
    public uint Field_0xF0 { get; set; }
    public uint Field_0xF4 { get; set; }
    public uint Field_0xF8 { get; set; }
    public uint Field_0xFC { get; set; }
    public uint Field_0x100 { get; set; }
    public uint Field_0x104 { get; set; }

    public virtual uint GetSize()
    {
        return 0x108;
    }

    public virtual void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        Type = (UIComponentType)bs.ReadUInt32();
        uint namesOffset = bs.ReadUInt32();
        uint namesCount = bs.ReadUInt32();
        SubType = bs.ReadUInt32();
        BlendMode = (BlendMode)bs.ReadUInt32();
        UnkColor_0x14 = Color.FromArgb(bs.ReadInt32());
        Unk_0x18 = bs.ReadUInt32();
        UnkPercentage0_0x1C = bs.ReadUInt32();
        UnkPercentage1 = bs.ReadUInt32();
        UnkPercentage2 = bs.ReadUInt32();
        Field_0x28 = bs.ReadInt32();
        Field_0x2C = bs.ReadSingle();
        Field_0x30 = bs.ReadSingle();
        Field_0x34 = bs.ReadSingle();
        Field_0x38 = bs.ReadSingle();
        Field_0x3C = bs.ReadSingle();
        Field_0x40 = bs.ReadSingle();
        Field_0x44 = bs.ReadSingle();
        Field_0x48 = bs.ReadSingle();
        Field_0x4C = bs.ReadInt32();
        Field_0x50 = bs.ReadSingle();
        Field_0x54 = bs.ReadSingle();
        Field_0x58 = bs.ReadSingle();
        Field_0x5C = bs.ReadSingle();
        Field_0x60 = bs.ReadInt16();
        Field_0x62 = bs.Read1Byte();
        Field_0x63 = bs.Read1Byte();
        Field_0x64 = bs.ReadSingle();
        Type_0x68 = bs.ReadUInt32();
        Field_0x6C = bs.ReadSingle();
        Field_0x70 = bs.ReadSingle();
        Unk_0x71 = bs.Read1Byte();
        Unk_0x72 = bs.Read1Byte();
        Unk_0x73 = bs.Read1Byte();
        Unk_0x74 = bs.Read1Byte();
        Field_0x78 = bs.ReadSingle();
        Field_0x7C = bs.ReadSingle();
        Field_0x80 = bs.ReadSingle();
        Field_0x84 = bs.ReadSingle();
        Field_0x88 = bs.ReadSingle();
        Type_0x8C = bs.ReadUInt32();
        Field_0x90 = bs.ReadSingle();
        Field_0x94 = bs.ReadSingle();
        UIAssetReference_0x98 = bs.ReadStructPointer<UIAssetReference>(basePos);
        Field_0x9C = bs.ReadSingle();
        Field_0xA0 = bs.ReadSingle();
        Field_0xA4 = bs.ReadSingle();
        uint offset_0xA8 = bs.ReadUInt32();
        uint unkUse_0xA8Field_0xAC = bs.ReadUInt32();
        Field_0xB0 = bs.ReadUInt32();
        Field_0xB4 = bs.ReadUInt32();
        Field_0xB8 = bs.ReadUInt32();
        Field_0xBC = bs.ReadSingle();
        Field_0xC0 = bs.ReadSingle();
        Field_0xC4 = bs.ReadSingle();
        Field_0xC8 = bs.ReadSingle();
        Field_0xCC = bs.ReadUInt32();
        Field_0xD0 = bs.ReadSingle();
        Field_0xD4 = bs.ReadSingle();
        Field_0xD8 = bs.ReadSingle();
        Field_0xDC = bs.ReadSingle();
        Field_0xE0 = bs.ReadSingle();
        Field_0xE4 = bs.ReadSingle();
        Field_0xE8 = bs.ReadSingle();
        Field_0xEC = bs.ReadSingle();
        Field_0xF0 = bs.ReadUInt32();
        Field_0xF4 = bs.ReadUInt32();
        Field_0xF8 = bs.ReadUInt32();
        Field_0xFC = bs.ReadUInt32();
        Field_0x100 = bs.ReadUInt32();
        Field_0x104 = bs.ReadUInt32();

        Names = bs.ReadStringsFromOffsetTable32(basePos + namesOffset, namesCount);

        if (unkUse_0xA8Field_0xAC >= 1)
        {
            bs.Position = basePos + offset_0xA8;
            Unk0xA8 = bs.ReadStruct<UIComponentUnk_0xA8>();
        }

        bs.Position = basePos + 0x108;
    }

    public void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();
    }

    public static UIComponentPropertiesBase Create(UIComponentType type)
    {
        return type switch
        {
            UIComponentType.Root => new UIComponentRoot(),
            UIComponentType.Custom => new UIComponentCustom(),
            UIComponentType.Button => new UIComponentButton(),
            UIComponentType.CheckBox => new UIComponentCheckBox(),
            UIComponentType.RadioButton => new UIComponentRadioButton(),
            UIComponentType.Tab => new UIComponentTab(),
            UIComponentType.Slider => new UIComponentSlider(),
            UIComponentType.ScrollBar => new UIComponentScrollBar(),
            UIComponentType.List => new UIComponentList(),
            UIComponentType.ListItem => new UIComponentListItem(),
            UIComponentType.Gauge => new UIComponentGauge(),
            UIComponentType.TextBoard => new UIComponentTextBoard(),
            UIComponentType.BahamutEffect => new UIComponentBahamutEffect(),
            _ => throw new NotImplementedException($"{type} not implemented")
        };
    }
}

public enum UIComponentType
{
    Root = 1,
    Custom = 2,
    Button = 3,
    CheckBox = 4,
    RadioButton = 5,
    Tab = 6,
    Slider = 7,
    ScrollBar = 8,
    List = 9,
    ListItem = 10,
    DropdownList = 11,
    Gauge = 12,
    TextBoard = 13,
    Window = 14,
    BahamutEffect = 15,
}

public enum BlendMode
{
    kNormal = 0,
    kDarken = 1,
    kMultiply = 2,
    kColorBurn = 3,
    kLinearBurn = 4,
    kLighten = 5,
    kScreen = 6,
    kColorDodge = 7,
    kLinearDodge = 8,
    kOverlay = 9,
    kSoftLight = 10,
    kHardLight = 11,
    kVividLight = 12,
    kLinearLight = 13,
    kHdrBlend = 14,
    kHdrLowBlend = 15,
    kHdrHighBlend = 16,
};