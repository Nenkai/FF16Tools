namespace FF16Tools.Files.CharaTimeline;

public class TimelineElement: BaseStruct
{
    public override int _totalSize => 0x20;

    public int field_0x00;
    public int UnkNameOffset;
    public int TimelineElemUnionTypeOrLayerId;
    public int FrameStart;
    public int NumFrames;
    public int field_0x14;
    public byte field_0x18;
    public byte field_0x19;
    public byte field_0x1A;
    public byte field_0x1B;
    public int Offset_0x1C;

    [RelativeField("UnkNameOffset")]
    public string Name;

    [RelativeField("Offset_0x1C")]
    public TimelineElementData DataUnion;
}
