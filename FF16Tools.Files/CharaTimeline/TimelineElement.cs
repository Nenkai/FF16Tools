namespace FF16Tools.Files.CharaTimeline;

public class TimelineElement: BaseStruct
{
    public override int _totalSize => 0x20;

    int field_0x00;

    [OffsetAttribute("Name")]
    int UnkNameOffset;

    int TimelineElemUnionTypeOrLayerId;
    int FrameStart;
    int NumFrames;
    int field_0x14;
    byte field_0x18;
    byte field_0x19;
    byte field_0x1A;
    byte field_0x1B;

    [OffsetAttribute("DataUnion", typeof(TimelineElementData))]
    int Offset_0x1C;
}
