using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class ControlPermission : TimelineElementBase, ISerializableStruct
{
    public ControlPermission()
    {
        UnionType = TimelineElementType.ControlPermission;
    }

    public byte Movement { get; set; }
    public byte MovmentInput { get; set; }
    public byte TurnPossible { get; set; }
    public byte Field_0x03 { get; set; }
    public byte ComboAttack { get; set; }
    public byte Field_0x05 { get; set; }
    public byte Jump { get; set; }
    public byte Dodge { get; set; }
    public byte BasicWeaponAction { get; set; }
    public byte UniqueWeaponAction { get; set; }
    public byte Shot { get; set; }
    public byte Summon { get; set; }
    public byte OtherAction { get; set; }
    public byte ChocoboAction { get; set; }
    public byte Field_0x0E { get; set; }
    public byte Field_0x0F { get; set; }
    public byte Field_0x10 { get; set; }
    public byte Field_0x11 { get; set; }
    public byte Field_0x12 { get; set; }
    public byte Field_0x13 { get; set; }
    public byte Field_0x14 { get; set; }
    public byte Field_0x15 { get; set; }
    public byte Field_0x16 { get; set; }
    public byte Field_0x17 { get; set; }
    public byte Field_0x18 { get; set; }
    public byte Field_0x19 { get; set; }
    public byte Field_0x1A { get; set; }
    public byte Field_0x1B { get; set; }
    public byte Field_0x1C { get; set; }
    public byte Field_0x1D { get; set; }
    public byte Field_0x1E { get; set; }
    public byte Field_0x1F { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Movement = bs.Read1Byte();
        MovmentInput = bs.Read1Byte();
        TurnPossible = bs.Read1Byte();
        Field_0x03 = bs.Read1Byte();
        ComboAttack = bs.Read1Byte();
        Field_0x05 = bs.Read1Byte();
        Jump = bs.Read1Byte();
        Dodge = bs.Read1Byte();
        BasicWeaponAction = bs.Read1Byte();
        UniqueWeaponAction = bs.Read1Byte();
        Shot = bs.Read1Byte();
        Summon = bs.Read1Byte();
        OtherAction = bs.Read1Byte();
        ChocoboAction = bs.Read1Byte();
        Field_0x0E = bs.Read1Byte();
        Field_0x0F = bs.Read1Byte();
        Field_0x10 = bs.Read1Byte();
        Field_0x11 = bs.Read1Byte();
        Field_0x12 = bs.Read1Byte();
        Field_0x13 = bs.Read1Byte();
        Field_0x14 = bs.Read1Byte();
        Field_0x15 = bs.Read1Byte();
        Field_0x16 = bs.Read1Byte();
        Field_0x17 = bs.Read1Byte();
        Field_0x18 = bs.Read1Byte();
        Field_0x19 = bs.Read1Byte();
        Field_0x1A = bs.Read1Byte();
        Field_0x1B = bs.Read1Byte();
        Field_0x1C = bs.Read1Byte();
        Field_0x1D = bs.Read1Byte();
        Field_0x1E = bs.Read1Byte();
        Field_0x1F = bs.Read1Byte();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteByte(Movement);
        bs.WriteByte(MovmentInput);
        bs.WriteByte(TurnPossible);
        bs.WriteByte(Field_0x03);
        bs.WriteByte(ComboAttack);
        bs.WriteByte(Field_0x05);
        bs.WriteByte(Jump);
        bs.WriteByte(Dodge);
        bs.WriteByte(BasicWeaponAction);
        bs.WriteByte(UniqueWeaponAction);
        bs.WriteByte(Shot);
        bs.WriteByte(Summon);
        bs.WriteByte(OtherAction);
        bs.WriteByte(ChocoboAction);
        bs.WriteByte(Field_0x0E);
        bs.WriteByte(Field_0x0F);
        bs.WriteByte(Field_0x10);
        bs.WriteByte(Field_0x11);
        bs.WriteByte(Field_0x12);
        bs.WriteByte(Field_0x13);
        bs.WriteByte(Field_0x14);
        bs.WriteByte(Field_0x15);
        bs.WriteByte(Field_0x16);
        bs.WriteByte(Field_0x17);
        bs.WriteByte(Field_0x18);
        bs.WriteByte(Field_0x19);
        bs.WriteByte(Field_0x1A);
        bs.WriteByte(Field_0x1B);
        bs.WriteByte(Field_0x1C);
        bs.WriteByte(Field_0x1D);
        bs.WriteByte(Field_0x1E);
        bs.WriteByte(Field_0x1F);
    }

    public uint GetSize() => 0x20;
}

