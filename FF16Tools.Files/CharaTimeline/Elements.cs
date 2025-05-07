using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline;
public class TimelineElement_5 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_5;

    public byte field_0x00;
    public byte field_0x01;
    public byte field_0x02;
    public byte field_0x03;
    public int field_0x07;
    public int field_0x0B;
    public int field_0x0F;

}


public class TimelineElement_8 : TimelineElementDataInner
{
    public class Sub8Struct : BaseStruct
    {
        public override int _totalSize => -1;

        public int DataOffset;
        public int DataSize;
        public float field_0x08;
        public float field_0x0C;

        // Technically relative but always written directly after the parent struct
        // Leave like that to include the data size in GetNonRelativeSize
        public byte[] Data;

        public override void Read(BinaryStream bs)
        {
            long startingPos = bs.Position;

            DataOffset = bs.ReadInt32();
            DataSize = bs.ReadInt32();
            field_0x08 = bs.ReadSingle();
            field_0x0C = bs.ReadSingle();

            long endPos = bs.Position;

            bs.Position = startingPos + DataOffset;
            Data = bs.ReadBytes(DataSize);

            bs.Position = endPos;
        }

        public override void Write(BinaryStream bs, Dictionary<(object, string), long> relativeFieldPos, Dictionary<string, long> stringPos)
        {
            bs.WriteInt32((int)(relativeFieldPos[(this, "Data")] - bs.Position)); // Data Offset
            bs.WriteInt32(Data.Length); // Data Size
            bs.WriteSingle(field_0x08);
            bs.WriteSingle(field_0x0C);

            // The data array was already written
        }
    }

    public override TimelineUnionType _elementType => TimelineUnionType.CameraAnimationRange;

    public int field_0x00;
    public int field_0x04;
    public byte field_0x08;
    public byte field_0x09;
    public byte field_0x0A;
    public byte field_0x0B;

    public Sub8Struct Entry1;
    public Sub8Struct Entry2;
    public Sub8Struct Entry3;
    public Sub8Struct Entry4;
    public Sub8Struct Entry5;
    public Sub8Struct Entry6;
    public Sub8Struct Entry7;
    public Sub8Struct Entry8;

    public int[] unks = new int[12];
    public int empty;

    public override void Write(BinaryStream bs, Dictionary<(object, string), long> relativeFieldPos, Dictionary<string, long> stringPos)
    {
        long startPos = bs.Position;
        // Move to end to write the Sub8Structs.Data arrays
        bs.Position += 8 * 16 + 8 + 4 + 4 + 4 * 12;
        relativeFieldPos[(Entry1, "Data")] = bs.Position;
        bs.WriteBytes(Entry1.Data);
        relativeFieldPos[(Entry2, "Data")] = bs.Position;
        bs.WriteBytes(Entry2.Data);
        relativeFieldPos[(Entry2, "Data")] = bs.Position;
        bs.WriteBytes(Entry3.Data);
        relativeFieldPos[(Entry3, "Data")] = bs.Position;
        bs.WriteBytes(Entry4.Data);
        relativeFieldPos[(Entry4, "Data")] = bs.Position;
        bs.WriteBytes(Entry5.Data);
        relativeFieldPos[(Entry5, "Data")] = bs.Position;
        bs.WriteBytes(Entry6.Data);
        relativeFieldPos[(Entry6, "Data")] = bs.Position;
        bs.WriteBytes(Entry7.Data);
        relativeFieldPos[(Entry7, "Data")] = bs.Position;
        bs.WriteBytes(Entry8.Data);
        relativeFieldPos[(Entry8, "Data")] = bs.Position;

        long DataEndPos = bs.Position;

        bs.Position = startPos;
        bs.WriteInt32(field_0x00);
        bs.WriteInt32(field_0x04);
        bs.WriteByte(field_0x08);
        bs.WriteByte(field_0x09);
        bs.WriteByte(field_0x0A);
        bs.WriteByte(field_0x0B);


        Entry1.Write(bs, relativeFieldPos, stringPos);
        Entry2.Write(bs, relativeFieldPos, stringPos);
        Entry3.Write(bs, relativeFieldPos, stringPos);
        Entry4.Write(bs, relativeFieldPos, stringPos);
        Entry5.Write(bs, relativeFieldPos, stringPos);
        Entry6.Write(bs, relativeFieldPos, stringPos);
        Entry7.Write(bs, relativeFieldPos, stringPos);
        Entry8.Write(bs, relativeFieldPos, stringPos);

        foreach (var i in unks)
        {
            bs.WriteInt32(i);
        }
        bs.WriteInt32(empty);

        bs.Position = DataEndPos;
    }
}

public class TimelineElement_9 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_9;

    public int AttackParamId;
    public int Field_0x04;
    public int Field_0x08;
    public int Field_0x0C;
    public int Field_0x10;
    public int Field_0x14;
    public int Field_0x18;
    public int Field_0x1C;
    public int Field_0x20;
    public int Field_0x24;
    public int Field_0x28;
    public int Field_0x2C;

}

public class TimelineElement_10 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.BattleCondition;

    public byte field_0x00;
    public byte field_0x01;
    public byte field_0x02;
    public byte field_0x03;
    public byte field_0x04;
    public byte field_0x05;
    public byte field_0x06;
    public byte field_0x07;
    public byte field_0x08;
    public byte field_0x09;
    public byte field_0x0A;
    public byte field_0x0B;
    public float field_0x0C;
    public int field_0x10;
    public float field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;

}


public class TimelineElement_12 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.BulletTimeRange;

    public float field_0x00;
    public float field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;

}


public class TimelineElement_17 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.ControlPermission;

    public byte field_0x00;
    public byte field_0x01;
    public byte field_0x02;
    public byte field_0x03;
    public byte field_0x04;
    public byte field_0x05;
    public byte field_0x06;
    public byte field_0x07;
    public byte field_0x08;
    public byte field_0x09;
    public byte field_0x0A;
    public byte field_0x0B;
    public byte field_0x0C;
    public byte field_0x0D;
    public byte field_0x0E;
    public byte field_0x0F;
    public byte field_0x10;
    public byte field_0x11;
    public byte field_0x12;
    public byte field_0x13;
    public byte field_0x14;
    public byte field_0x15;
    public byte field_0x16;
    public byte field_0x17;
    public byte field_0x18;
    public byte field_0x19;
    public byte field_0x1A;
    public byte field_0x1B;
    public byte field_0x1C;
    public byte field_0x1D;
    public byte field_0x1E;
    public byte field_0x1F;

}


public class TimelineElement_23 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_23;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;

}


public class TimelineElement_27 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.AdjustRootMoveRange;

    public int field_0x00;
    public int field_0x04;

}


public class TimelineElement_30 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_30;

    public int field_0x00;
    public int AnimPathOffset;
    public int field_0x08;
    public byte field_0x0C;
    public byte[] pad = new byte[3];
    public int field_0x10;
    public int field_0x14;
    public double field_0x18;
    public double field_0x20;
    public double field_0x28;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;
    public float field_0x3C;
    public byte field_0x40;
    public byte[] pad_ = new byte[3];
    public float field_0x44;
    public int field_0x48;
    public int field_0x4C;
    public int[] empty = new int[4];

    [RelativeField("AnimPathOffset")]
    public string SoundPath;
}


public class TimelineElement_31 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.PlaySoundTrigger;

    public int field_0x00;
    public int SoundPathOffset;
    public int field_0x08;
    public byte field_0x0C;
    public byte[] pad = new byte[3];
    public int field_0x10;
    public int field_0x14;
    public double field_0x18;
    public double field_0x20;
    public double field_0x28;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;
    public float field_0x3C;
    public byte field_0x40;
    public byte[] pad_ = new byte[3];
    public float field_0x44;
    public int field_0x48;
    public int field_0x4C;
    public int[] empty = new int[4];

    [RelativeField("SoundPathOffset")]
    public string Path;

}


public class TimelineElement_33 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.AttachWeaponTemporaryRange;

    public int field_0x00;
    public int field_0x04;

}


public class TimelineElement_45 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.ModelSE;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public float field_0x34;
    public int field_0x38;
    public float field_0x3C;
    public int field_0x40;
    public int field_0x44;
    public int field_0x48;
    public int field_0x4C;
    public int field_0x50;
    public int field_0x54;
    public double field_0x58;
    public int field_0x60;
    public int field_0x64;
    public double field_0x68;
    public int field_0x70;
    public int field_0x74;
    public int field_0x78;
    public float field_0x7C;
    public int field_0x80;
    public int field_0x84;
    public int field_0x88;
    public int field_0x8C;
    public int field_0x90;
    public int field_0x94;
    public int field_0x98;
    public int field_0x9C;

}


public class TimelineElement_47 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.BattleMessageRange;

    public int BattleMessageId;
    public int[] pad = new int[8];

}


public class TimelineElement_49 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_49;

    public int MSeqInputId;

}


public class TimelineElement_56 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_56;

    public int CameraFCurveId;
    public int UnkOffset1;
    public int field_0x08;
    public int UnkOffset2;
    public int field_0x10;
    public int UnkOffset3;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;

    [RelativeField("UnkOffset1", "UnionType")]
    public string Name1;
    [RelativeField("UnkOffset2", "UnionType")]
    public string Name2;
    [RelativeField("UnkOffset3", "UnionType")]
    public string Name3;
}


public class TimelineElement_57 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.PadVibration;

    public int CameraFCurveId;
    public int UnkOffset1;
    public int field_0x08;
    public int UnkOffset2;
    public int field_0x10;
    public int UnkOffset3;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;

    [RelativeField("UnkOffset1", "UnionType")]
    public string Name1;
    [RelativeField("UnkOffset2", "UnionType")]
    public string Name2;
    [RelativeField("UnkOffset3", "UnionType")]
    public string Name3;

}


public class TimelineElement_51 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.EnableDestructorCollision;

    public int AnimPathOffset;
    public int field_0x04;

    [RelativeField("AnimPathOffset", "UnionType")]
    public string Path;

}


public class TimelineElement_60 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_60;

    public int field_0x00;
    public int UnkName1Offset;
    public int field_0x08;
    public int UnkName2Offset;
    public int field_0x10;
    public int UnkName3Offset;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;
    public int field_0x3C;


    [RelativeField("UnkName1Offset", "UnionType")]
    public string Name1;
    [RelativeField("UnkName2Offset", "UnionType")]
    public string Name2;
    [RelativeField("UnkName3Offset", "UnionType")]
    public string Name3;


}


public class TimelineElement_73 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_73;

    public int field_0x00;
    public int UnkName1Offset;
    public int field_0x08;
    public int UnkName2Offset;
    public int field_0x10;
    public int UnkName3Offset;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;
    public int field_0x3C;


    [RelativeField("UnkName1Offset", "UnionType")]
    public string Name1;
    [RelativeField("UnkName2Offset", "UnionType")]
    public string Name2;
    [RelativeField("UnkName3Offset", "UnionType")]
    public string Name3;


}


public class TimelineElement_84 : TimelineElementDataInner
{
    public class Sub84Struct : BaseStruct
    {
        public override int _totalSize => -1;

        public int DataOffset;
        public int DataSize;
        public float field_0x08;
        public float field_0x0C;

        public byte[] Data;

        public override void Read(BinaryStream bs)
        {
            long startingPos = bs.Position;

            DataOffset = bs.ReadInt32();
            DataSize = bs.ReadInt32();
            field_0x08 = bs.ReadSingle();
            field_0x0C = bs.ReadSingle();

            long endPos = bs.Position;

            bs.Position = startingPos + DataOffset;
            Data = bs.ReadBytes(DataSize);

            bs.Position = endPos;
        }

        public override void Write(BinaryStream bs, Dictionary<(object, string), long> relativeFieldPos, Dictionary<string, long> stringPos)
        {
            bs.WriteInt32((int)(relativeFieldPos[(this, "Data")] - bs.Position)); // Data Offset
            bs.WriteInt32(Data.Length); // Data Size
            bs.WriteSingle(field_0x08);
            bs.WriteSingle(field_0x0C);

            // The data array was already written
        }
    }

    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_84;

    public Sub84Struct Entry1;
    public Sub84Struct Entry2;
    public Sub84Struct Entry3;
    public Sub84Struct Entry4;
    public Sub84Struct Entry5;
    public Sub84Struct Entry6;
    public Sub84Struct Entry7;
    public Sub84Struct Entry8;
    public Sub84Struct Entry9;

    public int Unk;
    public int[] unks = new int[4];
    public override void Write(BinaryStream bs, Dictionary<(object, string), long> relativeFieldPos, Dictionary<string, long> stringPos)
    {
        long startPos = bs.Position;
        // Move to end to write the Sub84Structs.Data arrays
        bs.Position += 9 * 16 + 4 + 4 * 4;
        relativeFieldPos[(Entry1, "Data")] = bs.Position;
        bs.WriteBytes(Entry1.Data);
        relativeFieldPos[(Entry2, "Data")] = bs.Position;
        bs.WriteBytes(Entry2.Data);
        relativeFieldPos[(Entry2, "Data")] = bs.Position;
        bs.WriteBytes(Entry3.Data);
        relativeFieldPos[(Entry3, "Data")] = bs.Position;
        bs.WriteBytes(Entry4.Data);
        relativeFieldPos[(Entry4, "Data")] = bs.Position;
        bs.WriteBytes(Entry5.Data);
        relativeFieldPos[(Entry5, "Data")] = bs.Position;
        bs.WriteBytes(Entry6.Data);
        relativeFieldPos[(Entry6, "Data")] = bs.Position;
        bs.WriteBytes(Entry7.Data);
        relativeFieldPos[(Entry7, "Data")] = bs.Position;
        bs.WriteBytes(Entry8.Data);
        relativeFieldPos[(Entry8, "Data")] = bs.Position;
        bs.WriteBytes(Entry9.Data);
        relativeFieldPos[(Entry9, "Data")] = bs.Position;
        bs.WriteBytes(Entry9.Data);

        long DataEndPos = bs.Position;

        bs.Position = startPos;
        Entry1.Write(bs, relativeFieldPos, stringPos);
        Entry2.Write(bs, relativeFieldPos, stringPos);
        Entry3.Write(bs, relativeFieldPos, stringPos);
        Entry4.Write(bs, relativeFieldPos, stringPos);
        Entry5.Write(bs, relativeFieldPos, stringPos);
        Entry6.Write(bs, relativeFieldPos, stringPos);
        Entry7.Write(bs, relativeFieldPos, stringPos);
        Entry8.Write(bs, relativeFieldPos, stringPos);
        Entry9.Write(bs, relativeFieldPos, stringPos);

        bs.WriteInt32(Unk);
        foreach (var i in unks)
        {
            bs.WriteInt32(i);
        }

        bs.Position = DataEndPos;
    }

}

public class TimelineElement_74 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.ControlRejectionRange;

    public int field_0x00;
    public byte field_0x04;
    public byte field_0x05;
    public byte field_0x06;
    public byte field_0x07;
    public byte field_0x08;
    public byte field_0x09;
    public byte field_0x0A;
    public byte field_0x0B;
    public byte field_0x0C;
    public byte field_0x0D;
    public byte field_0x0E;
    public byte field_0x0F;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;

}

public class TimelineElement_1001 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1001;

    public int field_0x00;
    public int AnimPathOffset;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;

    [RelativeField("AnimPathOffset")]
    public string Path;


}


public class TimelineElement_1002 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.Attack;

    public int AttackParamId;
    public int Name;
    public int field_0x08;
    public int field_0x0C;
    public int UnkName2Offset;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;

    [RelativeField("Name", "UnionType")]
    public string Path;
    [RelativeField("UnkName2Offset", "UnionType")]
    public string Path2;


}


public class TimelineElement_1004 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1004;

    public int field_0x00;
    public int field_0x04;
    public float field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;

}


public class TimelineElement_1005 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1005;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;

}


public class TimelineElement_1007 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1007;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
}


public class TimelineElement_1009 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.ComboEnable;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;

}


public class TimelineElement_1010 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.TurnToTarget;

    public int field_0x00;
    public int AnimPathOffset;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public float field_0x14;
    public float field_0x18;
    public int field_0x1C;
    public float field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;

    [RelativeField("AnimPathOffset")]
    public string Path;

}


public class TimelineElement_1012 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.MagicCreate;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;
    public int field_0x24;


}


public class TimelineElement_1014 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1014;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;

}


public class TimelineElement_1016 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.PrecedeInputUnk;

    public byte field_0x00;
    public byte field_0x01;
    public byte field_0x02;
    public byte field_0x03;
    public byte field_0x04;
    public byte field_0x05;
    public byte field_0x06;
    public byte field_0x07;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;

}


public class TimelineElement_1023 : TimelineElementDataInner
{
    public class Sub1023Struct : BaseStruct
    {
        public override int _totalSize => 0x58;

        public int Active;
        public int UnkIdSlot;
        public int EidId1;
        public int EidId2;
        public double field_0x10;
        public double field_0x18;
        public double field_0x20;
        public double field_0x28;
        public float field_0x30;
        public float field_0x34;
        public int[] pad = new int[8];
    }

    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1023;

    public int Offset_0x00;
    public int Count_0x00;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int VFXFileNameOffset;
    public int field_0x18;
    public int UnkNameOffset2;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;
    public int field_0x3C;
    public int field_0x40;
    public int field_0x44;
    public int field_0x48;
    public int field_0x4C;
    public int field_0x50;
    //public int field_0x54; - probably added to the definition by mistake

    [RelativeField("VFXFileNameOffset", "field_0x10")]
    public string VFXPath;
    [RelativeField("UnkNameOffset2", "field_0x18")]
    public string UnkName2;

    [RelativeField("Offset_0x00")]
    public List<Sub1023Struct> Sub;

    public override void Read(BinaryStream bs)
    {
        // this type is a bit too complex for the generic base method
        long startingPos = bs.Position;

        foreach (var field in GetAllFields())
        {
            if (field.FieldType == typeof(int))
                field.SetValue(this, bs.ReadInt32());
        }

        long finalPos = bs.Position;

        bs.Position = startingPos + 0x10 + VFXFileNameOffset;
        VFXPath = bs.ReadString(StringCoding.ZeroTerminated);
        bs.Position = startingPos + 0x18 + UnkNameOffset2;
        UnkName2 = bs.ReadString(StringCoding.ZeroTerminated);

        Sub = Timeline.ReadArrayOfStructs<Sub1023Struct>(bs, startingPos + Offset_0x00, Count_0x00);
        bs.Position = finalPos;
    }
}

public class TimelineElement_1030 : TimelineElementDataInner
{
    public class Sub1030Struct : BaseStruct
    {
        public override int _totalSize => 0x58;

        public int Active;
        public int UnkIdSlot;
        public int EidId1;
        public int EidId2;
        public double field_0x10;
        public double field_0x18;
        public double field_0x20;
        public double field_0x28;
        public float field_0x30;
        public float field_0x34;
        public int[] pad = new int[8];
    }

    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1030;

    public int Offset_0x00;
    public int Count_0x00;
    public int Offset_0x08;
    public int Count_0x08;
    public int field_0x10;
    public int VFXFileNameOffset;
    public int field_0x18;
    public int UnkNameOffset2;
    public int field_0x20;
    public byte field_0x24;
    public byte field_0x25;
    public byte field_0x26;
    public byte field_0x27;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int UnkNameOffset3;
    public int field_0x38;
    public int field_0x3C;
    public int field_0x40;
    public int field_0x44;
    public int field_0x48;
    public int field_0x4C;
    public int field_0x50;



    [RelativeField("VFXFileNameOffset", "field_0x10")]
    public string VFXPath;
    [RelativeField("UnkNameOffset2", "field_0x18")]
    public string UnkName2;
    [RelativeField("UnkNameOffset3", "UnionType")]
    public string UnkName3;

    [RelativeField("Offset_0x00")]
    public List<Sub1030Struct> Sub;

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
        }

        long finalPos = bs.Position;

        bs.Position = startingPos + 0x10 + VFXFileNameOffset;
        VFXPath = bs.ReadString(StringCoding.ZeroTerminated);
        bs.Position = startingPos + 0x18 + UnkNameOffset2;
        UnkName2 = bs.ReadString(StringCoding.ZeroTerminated);
        bs.Position = startingPos - 16 + UnkNameOffset3;
        UnkName3 = bs.ReadString(StringCoding.ZeroTerminated);

        Sub = Timeline.ReadArrayOfStructs<Sub1030Struct>(bs, startingPos + Offset_0x00, Count_0x00);
        bs.Position = finalPos;
    }
}

public class TimelineElement_1035 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1035;

    public int field_0x00;
    public int AnimPathOffset;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;


    [RelativeField("AnimPathOffset")]
    public string Path;


}


public class TimelineElement_1047 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.SummonPartsVisibleRange;

    public int SummonPartsPatternId;
    public float field_0x04;
    public float field_0x08;
    public byte field_0x0C;
    public byte field_0x0D;
    public byte field_0x0E;
    public byte field_0x0F;
    public int field_0x10;

}


public class TimelineElement_1049 : TimelineElementDataInner
{
    public class Sub1049Struct : BaseStruct
    {
        public override int _totalSize => 0x58;
        public int Active;
        public int UnkIdSlot;
        public int EidId1;
        public int EidId2;
        public double field_0x10;
        public double field_0x18;
        public double field_0x20;
        public double field_0x28;
        public float field_0x30;
        public float field_0x34;
        public int[] pad = new int[8];
    }

    public override int _totalSize => 0x70;
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1049;

    public int Offset_0x00;
    public int Count_0x00;
    public int Offset_0x08; // 0x1C Stride
    public int Count_0x08; // vfxexternallist
    public int field_0x10;
    public int VFXFileNameOffset;
    public int field_0x18;
    public int UnkNameOffset2;
    public int field_0x20;
    public byte field_0x24;
    public byte field_0x25;
    public byte field_0x26;
    public byte field_0x27;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;
    public int field_0x3C;
    public int field_0x40;
    public int field_0x44;
    public int field_0x48;
    public int field_0x4C;
    public int field_0x50;
    public int field_0x54;
    public int field_0x58;
    public int field_0x5C;
    public float field_0x60;
    public int field_0x64;
    public int field_0x68;
    public int field_0x6C;

    [RelativeField("VFXFileNameOffset", "field_0x10")]
    public string VFXPath;
    [RelativeField("UnkNameOffset2", "field_0x18")]
    public string UnkName2;

    [RelativeField("Offset_0x00")]
    public List<Sub1049Struct> Sub;

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
            else if (field.FieldType == typeof(float))
                field.SetValue(this, bs.ReadSingle());
        }

        long finalPos = bs.Position;

        bs.Position = startingPos + 0x10 + VFXFileNameOffset;
        VFXPath = bs.ReadString(StringCoding.ZeroTerminated);
        bs.Position = startingPos + 0x18 + UnkNameOffset2;
        UnkName2 = bs.ReadString(StringCoding.ZeroTerminated);

        Sub = Timeline.ReadArrayOfStructs<Sub1049Struct>(bs, startingPos + Offset_0x00, Count_0x00);
        bs.Position = finalPos;

        _leftoverData = bs.ReadBytes(_totalSize - (int)(finalPos - startingPos));
    }
}

public class TimelineElement_1053 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.BattleVoiceTrigger;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;

}


public class TimelineElement_1058 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.DisableReceiver;

    public int NameOffset;
    public int field_0x04;

    [RelativeField("NameOffset", "UnionType")]
    public string Path;
}


public class TimelineElement_1059 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1059;

    public int NameOffset;
    public float field_0x04;

    [RelativeField("NameOffset", "UnionType")]
    public string Path;
}


public class TimelineElement_1064 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1064;

    public int AttackParamId;
    public int Offset_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;

    [RelativeField("Offset_0x04", "UnionType")]
    public string Path;


}


public class TimelineElement_1066 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.MotionAttribute;

    public int field_0x00;

}


public class TimelineElement_1075 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1075;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;

}


public class TimelineElement_1084 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1084;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;

}


public class TimelineElement_1097 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.DisableCharaUnk;

    public int Offset_0x00;
    public int field_0x04;
    public int field_0x08;

    [RelativeField("Offset_0x00", "UnionType")]
    public string Path;


}


public class TimelineElement_1099 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1099;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public float field_0x14;
    public int field_0x18;
    public float field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;

}


public class TimelineElement_1102 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1102;

    public int Offset_0x00;
    public int field_0x04;

    [RelativeField("Offset_0x00", "UnionType")]
    public string Path;


}


public class TimelineElement_1103 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1103;

    public int Offset_0x00;
    public int field_0x04;

    [RelativeField("Offset_0x00", "UnionType")]
    public string Path;


}


public class TimelineElement_1107 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.StartCooldown; // not offical name

    public int Offset_0x00;
    public int field_0x04;

    [RelativeField("Offset_0x00", "UnionType")]
    public string Path;
}


public class TimelineElement_1115 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.kTimelineElem_1115;

    public int field_0x00;
    public int field_0x04;
    public float field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public float field_0x14;
    public int field_0x18;
    public float field_0x1C;
    public int field_0x20;
    public int field_0x24;
    public int field_0x28;
    public int field_0x2C;
    public int field_0x30;
    public int field_0x34;
    public int field_0x38;
    public int field_0x3C;
    public int field_0x40;
    public int field_0x44;
    public int field_0x48;
    public int field_0x4C;
    public int field_0x50;
    public int field_0x54;

}


public class TimelineElement_1117 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.StartCooldown;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;
    public int field_0x20;

}


public class TimelineElement_1130 : TimelineElementDataInner
{
    public override TimelineUnionType _elementType => TimelineUnionType.JustBuddyCommand;

    public int field_0x00;
    public int field_0x04;
    public int field_0x08;
    public int field_0x0C;
    public int field_0x10;
    public int field_0x14;
    public int field_0x18;
    public int field_0x1C;

}


