using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline;
public class TimelineElement_5 : TimelineElementDataInner
{
    public new string _elementTypeName = "5";

    byte field_0x00;
    byte field_0x01;
    byte field_0x02;
    byte field_0x03;
    int field_0x07;
    int field_0x0B;
    int field_0x0F;

}


public class TimelineElement_8 : TimelineElementDataInner
{
    public class Sub8Struct : BaseStruct
    {
        public override int _totalSize => 0x10;
        int DataOffset;
        int DataSize;
        float field_0x08;
        float field_0x0C;

        byte[] Data;

        public override void Read(BinaryStream bs)
        {
            long startingPos = bs.Position;

            DataOffset = bs.ReadInt32();
            DataSize = bs.ReadInt32();
            field_0x08 = bs.ReadSingle();
            field_0x0C = bs.ReadSingle();

            bs.Position = startingPos + DataOffset;
            Data = bs.ReadBytes(DataSize);

            bs.Position = startingPos + _totalSize;
        }
    }

    public new string _elementTypeName = "CameraAnimationRange";

    int field_0x00;
    int field_0x04;
    byte field_0x08;
    byte field_0x09;
    byte field_0x0A;
    byte field_0x0B;

    Sub8Struct Entry1;
    Sub8Struct Entry2;
    Sub8Struct Entry3;
    Sub8Struct Entry4;
    Sub8Struct Entry5;
    Sub8Struct Entry6;
    Sub8Struct Entry7;
    Sub8Struct Entry8;

    int[] unks = new int[12];
}

public class TimelineElement_10 : TimelineElementDataInner
{
    public new string _elementTypeName = "BattleCondition";

    byte field_0x00;
    byte field_0x01;
    byte field_0x02;
    byte field_0x03;
    byte field_0x04;
    byte field_0x05;
    byte field_0x06;
    byte field_0x07;
    byte field_0x08;
    byte field_0x09;
    byte field_0x0A;
    byte field_0x0B;
    float field_0x0C;
    int field_0x10;
    float field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;

}


public class TimelineElement_12 : TimelineElementDataInner
{
    public new string _elementTypeName = "BulletTimeRange";

    float field_0x00;
    float field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;

}


public class TimelineElement_17 : TimelineElementDataInner
{
    public new string _elementTypeName = "ControlPermission";

    byte field_0x00;
    byte field_0x01;
    byte field_0x02;
    byte field_0x03;
    byte field_0x04;
    byte field_0x05;
    byte field_0x06;
    byte field_0x07;
    byte field_0x08;
    byte field_0x09;
    byte field_0x0A;
    byte field_0x0B;
    byte field_0x0C;
    byte field_0x0D;
    byte field_0x0E;
    byte field_0x0F;
    byte field_0x10;
    byte field_0x11;
    byte field_0x12;
    byte field_0x13;
    byte field_0x14;
    byte field_0x15;
    byte field_0x16;
    byte field_0x17;
    byte field_0x18;
    byte field_0x19;
    byte field_0x1A;
    byte field_0x1B;
    byte field_0x1C;
    byte field_0x1D;
    byte field_0x1E;
    byte field_0x1F;

}


public class TimelineElement_23 : TimelineElementDataInner
{
    public new string _elementTypeName = "23";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;

}


public class TimelineElement_27 : TimelineElementDataInner
{
    public new string _elementTypeName = "27";

    int field_0x00;
    int field_0x04;

}


public class TimelineElement_30 : TimelineElementDataInner
{
    public new string _elementTypeName = "30";

    int field_0x00;
    [OffsetAttribute("SoundPath")]
    int AnimPathOffset;
    int field_0x08;
    byte field_0x0C;
    byte[] pad = new byte[3];
    int field_0x10;
    int field_0x14;
    double field_0x18;
    double field_0x20;
    double field_0x28;
    int field_0x30;
    int field_0x34;
    int field_0x38;
    float field_0x3C;
    byte field_0x40;
    byte[] pad_ = new byte[3];
    float field_0x44;
    int field_0x48;
    int field_0x4C;
    int[] empty = new int[4];
}


public class TimelineElement_31 : TimelineElementDataInner
{
    public new string _elementTypeName = "PlaySoundTrigger";

    int field_0x00;
    [OffsetAttribute("Path")]
    int SoundPathOffset;
    int field_0x08;
    byte field_0x0C;
    byte[] pad = new byte[3];
    int field_0x10;
    int field_0x14;
    double field_0x18;
    double field_0x20;
    double field_0x28;
    int field_0x30;
    int field_0x34;
    int field_0x38;
    float field_0x3C;
    byte field_0x40;
    byte[] pad_ = new byte[3];
    float field_0x44;
    int field_0x48;
    int field_0x4C;
    int[] empty = new int[4];

}


public class TimelineElement_33 : TimelineElementDataInner
{
    public new string _elementTypeName = "AttachWeaponTemporaryRange";

    int field_0x00;
    int field_0x04;

}


public class TimelineElement_45 : TimelineElementDataInner
{
    public new string _elementTypeName = "ModelSE";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    float field_0x34;
    int field_0x38;
    float field_0x3C;
    int field_0x40;
    int field_0x44;
    int field_0x48;
    int field_0x4C;
    int field_0x50;
    int field_0x54;
    double field_0x58;
    int field_0x60;
    int field_0x64;
    double field_0x68;
    int field_0x70;
    int field_0x74;
    int field_0x78;
    float field_0x7C;
    int field_0x80;
    int field_0x84;
    int field_0x88;
    int field_0x8C;
    int field_0x90;
    int field_0x94;
    int field_0x98;
    int field_0x9C;

}


public class TimelineElement_47 : TimelineElementDataInner
{
    public new string _elementTypeName = "BattleMessageRange";

    int BattleMessageId;
    int[] pad = new int[8];

}


public class TimelineElement_49 : TimelineElementDataInner
{
    public new string _elementTypeName = "49";

    int MSeqInputId;

}


public class TimelineElement_56 : TimelineElementDataInner
{
    public new string _elementTypeName = "56";

    int CameraFCurveId;
    [OffsetAttribute("Name1", relativeField: "UnionType")]
    int UnkOffset1;
    int field_0x08;
    [OffsetAttribute("Name2", relativeField: "UnionType")]
    int UnkOffset2;
    int field_0x10;
    [OffsetAttribute("Name3", relativeField: "UnionType")]
    int UnkOffset3;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    int field_0x34;
}


public class TimelineElement_57 : TimelineElementDataInner
{
    public new string _elementTypeName = "PadVibration";

    int CameraFCurveId;
    [OffsetAttribute("Name1", relativeField:"UnionType")]
    int UnkOffset1;
    int field_0x08;
    [OffsetAttribute("Name2", relativeField: "UnionType")]
    int UnkOffset2;
    int field_0x10;
    [OffsetAttribute("Name3", relativeField: "UnionType")]
    int UnkOffset3;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    int field_0x34;

}


public class TimelineElement_51 : TimelineElementDataInner
{
    public new string _elementTypeName = "EnableDestructorCollision";

    [OffsetAttribute("Path", relativeField: "UnionType")]
    int AnimPathOffset;
    int field_0x04;


}


public class TimelineElement_60 : TimelineElementDataInner
{
    public new string _elementTypeName = "60";

    int field_0x00;
    [OffsetAttribute("Name1", relativeField: "UnionType")]
    int UnkName1Offset;
    int field_0x08;
    [OffsetAttribute("Name2", relativeField: "UnionType")]
    int UnkName2Offset;
    int field_0x10;
    [OffsetAttribute("Name3", relativeField: "UnionType")]
    int UnkName3Offset;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    int field_0x34;
    int field_0x38;
    int field_0x3C;


}


public class TimelineElement_73 : TimelineElementDataInner
{
    public new string _elementTypeName = "73";

    int field_0x00;
    [OffsetAttribute("Name1", relativeField: "UnionType")]
    int UnkName1Offset;
    int field_0x08;
    [OffsetAttribute("Name2", relativeField: "UnionType")]
    int UnkName2Offset;
    int field_0x10;
    [OffsetAttribute("Name3", relativeField: "UnionType")]
    int UnkName3Offset;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    int field_0x34;
    int field_0x38;
    int field_0x3C;


}


public class TimelineElement_84 : TimelineElementDataInner
{
    public class Sub84Struct : BaseStruct
    {
        public override int _totalSize => 0x10;
        int DataOffset;
        int DataSize;
        float field_0x08;
        float field_0x0C;

        byte[] Data;

        public override void Read(BinaryStream bs)
        {
            long startingPos = bs.Position;

            DataOffset = bs.ReadInt32();
            DataSize = bs.ReadInt32();
            field_0x08 = bs.ReadSingle();
            field_0x0C = bs.ReadSingle();

            bs.Position = startingPos + DataOffset;
            Data = bs.ReadBytes(DataSize);

            bs.Position = startingPos + _totalSize;
        }
    }

    public new string _elementTypeName = "84";

    Sub84Struct Entry1;
    Sub84Struct Entry2;
    Sub84Struct Entry3;
    Sub84Struct Entry4;
    Sub84Struct Entry5;
    Sub84Struct Entry6;
    Sub84Struct Entry7;
    Sub84Struct Entry8;
    Sub84Struct Entry9;

    int Unk;
    int[] unks = new int[4];

}

public class TimelineElement_74 : TimelineElementDataInner
{
    public new string _elementTypeName = "ControlRejectionRange";

    int field_0x00;
    byte field_0x04;
    byte field_0x05;
    byte field_0x06;
    byte field_0x07;
    byte field_0x08;
    byte field_0x09;
    byte field_0x0A;
    byte field_0x0B;
    byte field_0x0C;
    byte field_0x0D;
    byte field_0x0E;
    byte field_0x0F;
    int field_0x10;
    int field_0x14;
    int field_0x18;

}


public class TimelineElement_1001 : TimelineElementDataInner
{
    public new string _elementTypeName = "1001";

    int field_0x00;
    [OffsetAttribute("Path")]
    int AnimPathOffset;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;
    int field_0x28;


}


public class TimelineElement_1002 : TimelineElementDataInner
{
    public new string _elementTypeName = "1002";

    int AttackParamId;
    [OffsetAttribute("Path", relativeField: "UnionType")]
    int Name;
    int field_0x08;
    int field_0x0C;
    [OffsetAttribute("Path2", relativeField: "UnionType")]
    int UnkName2Offset;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;


}


public class TimelineElement_1004 : TimelineElementDataInner
{
    public new string _elementTypeName = "1004";

    int field_0x00;
    int field_0x04;
    float field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;

}


public class TimelineElement_1005 : TimelineElementDataInner
{
    public new string _elementTypeName = "1005";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;

}


public class TimelineElement_1007 : TimelineElementDataInner
{
    public new string _elementTypeName = "1007";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
}


public class TimelineElement_1009 : TimelineElementDataInner
{
    public new string _elementTypeName = "1009";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;

}


public class TimelineElement_1010 : TimelineElementDataInner
{
    public new string _elementTypeName = "TurnToTarget";

    int field_0x00;
    [OffsetAttribute("Path")]
    int AnimPathOffset;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    float field_0x14;
    float field_0x18;
    int field_0x1C;
    float field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    int field_0x34;
    int field_0x38;

}


public class TimelineElement_1012 : TimelineElementDataInner
{
    public new string _elementTypeName = "MagicCreate";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;
    int field_0x24;


}


public class TimelineElement_1014 : TimelineElementDataInner
{
    public new string _elementTypeName = "1014";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;

}


public class TimelineElement_1016 : TimelineElementDataInner
{
    public new string _elementTypeName = "PrecedeInputUnk";

    byte field_0x00;
    byte field_0x01;
    byte field_0x02;
    byte field_0x03;
    byte field_0x04;
    byte field_0x05;
    byte field_0x06;
    byte field_0x07;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;

}


public class TimelineElement_1023 : TimelineElementDataInner
{
    public class Sub1023Struct : BaseStruct
    {
        public override int _totalSize => 0x58;

        int Active;
        int UnkIdSlot;
        int EidId1;
        int EidId2;
        double field_0x10;
        double field_0x18;
        double field_0x20;
        double field_0x28;
        float field_0x30;
        float field_0x34;
        int[] pad = new int[8];
    }

    public new string _elementTypeName = "1023";

    int Offset_0x00;
    int Count_0x00;
    int field_0x08;
    int field_0x0C;

    int field_0x10;

    [OffsetAttribute("VFXPath", relativeField: nameof(field_0x10))]
    int VFXFileNameOffset;

    int field_0x18;

    [OffsetAttribute("UnkName2", relativeField: nameof(field_0x18))]
    int UnkNameOffset2;

    int field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    int field_0x34;
    int field_0x38;
    int field_0x3C;
    int field_0x40;
    int field_0x44;
    int field_0x48;
    int field_0x4C;
    int field_0x50;
    int field_0x54;

    public override void Read(BinaryStream bs)
    {
        // this type is a bit too complex for the generic base method
        long startingPos = bs.Position;

        foreach (var field in GetAllFields())
        {
            field.SetValue(this, bs.ReadInt32());
        }

        long finalPos = bs.Position;

        bs.Position = startingPos + 0x10 + VFXFileNameOffset;
        _referencedStrings["VFXPath"] = bs.ReadString(StringCoding.ZeroTerminated);
        bs.Position = startingPos + 0x18 + UnkNameOffset2;
        _referencedStrings["UnkName2"] = bs.ReadString(StringCoding.ZeroTerminated);

        _referencedArrays["Sub"] = Timeline.ReadArrayOfStructs<Sub1023Struct>(bs, startingPos + Offset_0x00, Count_0x00).Select(s => (BaseStruct)s).ToList();
        bs.Position = finalPos;
    }
}

public class TimelineElement_1030 : TimelineElementDataInner
{
    public class Sub1030Struct : BaseStruct
    {
        public override int _totalSize => 0x54;

        int Active;
        int UnkIdSlot;
        int EidId1;
        int EidId2;
        double field_0x10;
        double field_0x18;
        double field_0x20;
        double field_0x28;
        float field_0x30;
        float field_0x34;
        int[] pad = new int[8];
    }

    public new string _elementTypeName = "1030";

    int Offset_0x00;
    int Count_0x00;
    int Offset_0x08;
    int Count_0x08;

    int field_0x10;
    [OffsetAttribute("VFXPath", relativeField: nameof(field_0x10))]
    int VFXFileNameOffset;

    int field_0x18;
    [OffsetAttribute("UnkName2", relativeField: nameof(field_0x18))]
    int UnkNameOffset2;

    int field_0x20;
    byte field_0x24;
    byte field_0x25;
    byte field_0x26;
    byte field_0x27;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    [OffsetAttribute("UnkName3", relativeField: "UnionType")]
    int UnkNameOffset3;
    int field_0x38;
    int field_0x3C;
    int field_0x40;
    int field_0x44;
    int field_0x48;
    int field_0x4C;
    int field_0x50;

    public override void Read(BinaryStream bs)
    {
        // this type is a bit too complex for the generic base method
        long startingPos = bs.Position;

        foreach (var field in GetAllFields())
        {
            if (field.FieldType == typeof(int))
                field.SetValue(this, bs.ReadInt32());
            else
                field.SetValue(this, bs.Read1Byte());
        }

        long finalPos = bs.Position;

        bs.Position = startingPos + 0x10 + VFXFileNameOffset;
        _referencedStrings["VFXPath"] = bs.ReadString(StringCoding.ZeroTerminated);
        bs.Position = startingPos + 0x18 + UnkNameOffset2;
        _referencedStrings["UnkName2"] = bs.ReadString(StringCoding.ZeroTerminated);
        bs.Position = startingPos + UnkNameOffset3;
        _referencedStrings["UnkName3"] = bs.ReadString(StringCoding.ZeroTerminated);

        _referencedArrays["Sub"] = Timeline.ReadArrayOfStructs<Sub1030Struct>(bs, startingPos + Offset_0x00, Count_0x00).Select(s => (BaseStruct)s).ToList();
        bs.Position = finalPos;
    }
}

public class TimelineElement_1035 : TimelineElementDataInner
{
    public new string _elementTypeName = "1035";

    int field_0x00;
    [OffsetAttribute("Path")]
    int AnimPathOffset;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;


}


public class TimelineElement_1047 : TimelineElementDataInner
{
    public new string _elementTypeName = "SummonPartsVisibleRange";

    int SummonPartsPatternId;
    float field_0x04;
    float field_0x08;
    byte field_0x0C;
    byte field_0x0D;
    byte field_0x0E;
    byte field_0x0F;
    int field_0x10;

}


public class TimelineElement_1049 : TimelineElementDataInner
{
    public class Sub1049Struct : BaseStruct
    {
        public override int _totalSize => 0x58;
        int Active;
        int UnkIdSlot;
        int EidId1;
        int EidId2;
        double field_0x10;
        double field_0x18;
        double field_0x20;
        double field_0x28;
        float field_0x30;
        float field_0x34;
        int[] pad = new int[8];
    }

    public override int _totalSize => 0x70;
    public new string _elementTypeName = "1049";

    int Offset_0x00;
    int Count_0x00;
    int Offset_0x08; // 0x1C Stride
    int Count_0x08; // vfxexternallist

    int field_0x10;
    [OffsetAttribute("VFXPath", relativeField: nameof(field_0x10))]
    int VFXFileNameOffset;

    int field_0x18;
    [OffsetAttribute("UnkName2", relativeField: nameof(field_0x18))]
    int UnkNameOffset2;

    int field_0x20;
    byte field_0x24;
    byte field_0x25;
    byte field_0x26;
    byte field_0x27;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    int field_0x34;
    int field_0x38;
    int field_0x3C;
    int field_0x40;
    int field_0x44;
    int field_0x48;
    int field_0x4C;
    int field_0x50;
    int field_0x54;
    int field_0x58;
    int field_0x5C;
    float field_0x60;
    int field_0x64;
    int field_0x68;
    int field_0x6C;

    public override void Read(BinaryStream bs)
    {
        // this type is a bit too complex for the generic base method
        long startingPos = bs.Position;

        foreach (var field in GetAllFields())
        {
            if (field.FieldType == typeof(int))
                field.SetValue(this, bs.ReadInt32());
            else if (field.FieldType == typeof(byte))
                field.SetValue(this, bs.Read1Byte());
            else
                field.SetValue(this, bs.ReadSingle());
        }

        long finalPos = bs.Position;

        bs.Position = startingPos + 0x10 + VFXFileNameOffset;
        _referencedStrings["VFXPath"] = bs.ReadString(StringCoding.ZeroTerminated);
        bs.Position = startingPos + 0x18 + UnkNameOffset2;
        _referencedStrings["UnkName2"] = bs.ReadString(StringCoding.ZeroTerminated);

        _referencedArrays["Sub"] = Timeline.ReadArrayOfStructs<Sub1049Struct>(bs, startingPos + Offset_0x00, Count_0x00).Select(s => (BaseStruct)s).ToList();
        bs.Position = finalPos;

        _leftoverData = bs.ReadBytes(_totalSize - (int)(finalPos - startingPos));
    }
}

public class TimelineElement_1053 : TimelineElementDataInner
{
    public new string _elementTypeName = "BattleVoiceTrigger";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;

}


public class TimelineElement_1058 : TimelineElementDataInner
{
    public new string _elementTypeName = "DisableReceiver";

    [OffsetAttribute("Path", relativeField: "UnionType")]
    int NameOffset;
    int field_0x04;
}


public class TimelineElement_1059 : TimelineElementDataInner
{
    public new string _elementTypeName = "1059";

    [OffsetAttribute("Path", relativeField: "UnionType")]
    int NameOffset;
    float field_0x04;
}


public class TimelineElement_1064 : TimelineElementDataInner
{
    public new string _elementTypeName = "1064";

    int field_0x00;
    [OffsetAttribute("Path", relativeField: "UnionType")]
    int Offset_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;


}


public class TimelineElement_1066 : TimelineElementDataInner
{
    public new string _elementTypeName = "1066";

    int field_0x00;

}


public class TimelineElement_1075 : TimelineElementDataInner
{
    public new string _elementTypeName = "1075";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;

}


public class TimelineElement_1084 : TimelineElementDataInner
{
    public new string _elementTypeName = "1084";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;

}


public class TimelineElement_1097 : TimelineElementDataInner
{
    public new string _elementTypeName = "DisableCharaUnk";

    [OffsetAttribute("Path", relativeField: "UnionType")]
    int Offset_0x00;
    int field_0x04;
    int field_0x08;


}


public class TimelineElement_1099 : TimelineElementDataInner
{
    public new string _elementTypeName = "1099";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    float field_0x14;
    int field_0x18;
    float field_0x1C;
    int field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;

}


public class TimelineElement_1102 : TimelineElementDataInner
{
    public new string _elementTypeName = "1102";

    [OffsetAttribute("Path", relativeField: "UnionType")]
    int Offset_0x00;
    int field_0x04;


}


public class TimelineElement_1103 : TimelineElementDataInner
{
    public new string _elementTypeName = "1103";

    [OffsetAttribute("Path", relativeField: "UnionType")]
    int Offset_0x00;
    int field_0x04;


}


public class TimelineElement_1107 : TimelineElementDataInner
{
    public new string _elementTypeName = "1107";

    [OffsetAttribute("Path", relativeField: "UnionType")]
    int Offset_0x00;
    int field_0x04;


}


public class TimelineElement_1115 : TimelineElementDataInner
{
    public new string _elementTypeName = "1115";

    int field_0x00;
    int field_0x04;
    float field_0x08;
    int field_0x0C;
    int field_0x10;
    float field_0x14;
    int field_0x18;
    float field_0x1C;
    int field_0x20;
    int field_0x24;
    int field_0x28;
    int field_0x2C;
    int field_0x30;
    int field_0x34;
    int field_0x38;
    int field_0x3C;
    int field_0x40;
    int field_0x44;
    int field_0x48;
    int field_0x4C;
    int field_0x50;
    int field_0x54;

}


public class TimelineElement_1117 : TimelineElementDataInner
{
    public new string _elementTypeName = "1117";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;
    int field_0x20;

}


public class TimelineElement_1130 : TimelineElementDataInner
{
    public new string _elementTypeName = "1130";

    int field_0x00;
    int field_0x04;
    int field_0x08;
    int field_0x0C;
    int field_0x10;
    int field_0x14;
    int field_0x18;
    int field_0x1C;

}


