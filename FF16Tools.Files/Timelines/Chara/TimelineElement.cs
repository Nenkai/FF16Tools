namespace FF16Tools.Files.Timelines.Chara;

public class TimelineElement : ISerializableStruct
{
    /// <summary>
    /// Whether to not throw when an unknown unsupported element is attempted to be read.
    /// </summary>
    public static bool SkipOnUnknownElement { get; set; } = false;

    public uint Field_0x00 { get; set; }
    public string? Name { get; set; }
    public TimelineElementType ElementType { get; set; }

    /// <summary>
    /// Frame number within the timeline in which this element starts.
    /// </summary>
    public uint FrameStart { get; set; }

    /// <summary>
    /// Number of frames this element lasts for. There are 30 frames per second.
    /// </summary>
    public uint NumFrames { get; set; }
    public uint Field_0x14 { get; set; }
    public byte Field_0x18 { get; set; }
    public byte Field_0x19 { get; set; }
    public byte Field_0x1A { get; set; }
    public byte Field_0x1B { get; set; }
    public TimelineElementBase? DataUnion { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        long thisPos = bs.Position;

        Field_0x00 = bs.ReadUInt32();
        Name = bs.ReadStringPointer(thisPos);
        ElementType = (TimelineElementType)bs.ReadUInt32();
        FrameStart = bs.ReadUInt32();
        NumFrames = bs.ReadUInt32();
        Field_0x14 = bs.ReadUInt32();
        Field_0x18 = bs.Read1Byte();
        Field_0x19 = bs.Read1Byte();
        Field_0x1A = bs.Read1Byte();
        Field_0x1B = bs.Read1Byte();
        int dataHdrOffset = bs.ReadInt32();

        bs.Position = thisPos + dataHdrOffset;
        try
        {
            DataUnion = TimelineElementFactory.CreateElement(ElementType);
        }
        catch (Exception)
        {
            if (!SkipOnUnknownElement)
                throw;
        }

        DataUnion?.Read(bs);
    }

    /// <summary>
    /// Note: won't write data, only header.
    /// </summary>
    /// <param name="bs"></param>
    public void Write(SmartBinaryStream bs)
    {
        long thisPos = bs.Position;

        bs.WriteUInt32(Field_0x00);
        bs.AddStringPointer(Name, thisPos);
        bs.WriteUInt32((uint)ElementType);
        bs.WriteUInt32(FrameStart);
        bs.WriteUInt32(NumFrames);
        bs.WriteUInt32(Field_0x14);
        bs.WriteByte(Field_0x18);
        bs.WriteByte(Field_0x19);
        bs.WriteByte(Field_0x1A);
        bs.WriteByte(Field_0x1B);
        bs.WriteObjectPointer(DataUnion, thisPos);
    }

    public uint GetSize() => 0x20;
}
