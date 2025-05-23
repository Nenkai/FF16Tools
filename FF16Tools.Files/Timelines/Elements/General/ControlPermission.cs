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

    public bool Movement { get; set; }
    public bool MovmentInput { get; set; }
    public bool TurnPossible { get; set; }
    public bool Field_0x03 { get; set; }
    public bool ComboAttack { get; set; }
    public bool Field_0x05 { get; set; }
    public bool Jump { get; set; }
    public bool Dodge { get; set; }
    public bool BasicWeaponAction { get; set; }
    public bool UniqueWeaponAction { get; set; }
    public bool Shot { get; set; }
    public bool Summon { get; set; }
    public bool OtherAction { get; set; }
    public bool ChocoboAction { get; set; }
    public byte Field_0x0E { get; set; }
    public byte Field_0x0F { get; set; }
    public bool Field_0x10 { get; set; }
    public bool Field_0x11 { get; set; }
    public byte Field_0x12 { get; set; }
    public bool Field_0x13 { get; set; }
    public bool Field_0x14 { get; set; }
    public bool Field_0x15 { get; set; }
    public bool Field_0x16 { get; set; }
    public bool Field_0x17 { get; set; }
    public bool Field_0x18 { get; set; }
    public bool Field_0x19 { get; set; }
    public bool Field_0x1A { get; set; }
    public bool Field_0x1B { get; set; }
    public bool Field_0x1C { get; set; }
    public bool Field_0x1D { get; set; }
    public bool Field_0x1E { get; set; }
    public bool Field_0x1F { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        Movement = bs.ReadBoolean();
        MovmentInput = bs.ReadBoolean();
        TurnPossible = bs.ReadBoolean();
        Field_0x03 = bs.ReadBoolean();
        ComboAttack = bs.ReadBoolean();
        Field_0x05 = bs.ReadBoolean();
        Jump = bs.ReadBoolean();
        Dodge = bs.ReadBoolean();
        BasicWeaponAction = bs.ReadBoolean();
        UniqueWeaponAction = bs.ReadBoolean();
        Shot = bs.ReadBoolean();
        Summon = bs.ReadBoolean();
        OtherAction = bs.ReadBoolean();
        ChocoboAction = bs.ReadBoolean();
        Field_0x0E = bs.Read1Byte();
        Field_0x0F = bs.Read1Byte();
        Field_0x10 = bs.ReadBoolean();
        Field_0x11 = bs.ReadBoolean();
        Field_0x12 = bs.Read1Byte();
        Field_0x13 = bs.ReadBoolean();
        Field_0x14 = bs.ReadBoolean();
        Field_0x15 = bs.ReadBoolean();
        Field_0x16 = bs.ReadBoolean();
        Field_0x17 = bs.ReadBoolean();
        Field_0x18 = bs.ReadBoolean();
        Field_0x19 = bs.ReadBoolean();
        Field_0x1A = bs.ReadBoolean();
        Field_0x1B = bs.ReadBoolean();
        Field_0x1C = bs.ReadBoolean();
        Field_0x1D = bs.ReadBoolean();
        Field_0x1E = bs.ReadBoolean();
        Field_0x1F = bs.ReadBoolean();
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteBoolean(Movement);
        bs.WriteBoolean(MovmentInput);
        bs.WriteBoolean(TurnPossible);
        bs.WriteBoolean(Field_0x03);
        bs.WriteBoolean(ComboAttack);
        bs.WriteBoolean(Field_0x05);
        bs.WriteBoolean(Jump);
        bs.WriteBoolean(Dodge);
        bs.WriteBoolean(BasicWeaponAction);
        bs.WriteBoolean(UniqueWeaponAction);
        bs.WriteBoolean(Shot);
        bs.WriteBoolean(Summon);
        bs.WriteBoolean(OtherAction);
        bs.WriteBoolean(ChocoboAction);
        bs.WriteByte(Field_0x0E);
        bs.WriteByte(Field_0x0F);
        bs.WriteBoolean(Field_0x10);
        bs.WriteBoolean(Field_0x11);
        bs.WriteByte(Field_0x12);
        bs.WriteBoolean(Field_0x13);
        bs.WriteBoolean(Field_0x14);
        bs.WriteBoolean(Field_0x15);
        bs.WriteBoolean(Field_0x16);
        bs.WriteBoolean(Field_0x17);
        bs.WriteBoolean(Field_0x18);
        bs.WriteBoolean(Field_0x19);
        bs.WriteBoolean(Field_0x1A);
        bs.WriteBoolean(Field_0x1B);
        bs.WriteBoolean(Field_0x1C);
        bs.WriteBoolean(Field_0x1D);
        bs.WriteBoolean(Field_0x1E);
        bs.WriteBoolean(Field_0x1F);
    }

    public uint GetSize() => 0x20;
}

