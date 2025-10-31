using Syroot.BinaryData;

namespace FF16Tools.Files.Timelines.Chara;

public class CharaTimelineFile
{
    public const int MAGIC = 0x4C544346;
    public uint Version { get; set; }
    public Timeline? Timeline { get; set; }

    public static CharaTimelineFile Open(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        using var fs = File.OpenRead(file);
        var charaTimelineFile = new CharaTimelineFile();
        charaTimelineFile.Read(fs);
        return charaTimelineFile;
    }

    public void Read(string file)
    {
        using var fs = File.OpenRead(file);
        Read(fs);
    }

    public void Read(Stream stream)
    {
        var bs = new SmartBinaryStream(stream, stringCoding: StringCoding.ZeroTerminated);

        if (bs.ReadUInt32() != MAGIC)
            throw new InvalidDataException("Not a chara timeline (.tlb) file. Magic did not match.");

        Version = bs.ReadUInt32();
        bs.ReadCheckPadding(0x10);
        int timelineDataPosition = bs.ReadInt32();

        if (timelineDataPosition != 0)
        {
            bs.Position = timelineDataPosition;
            Timeline = new Timeline();
            Timeline.Read(bs);
        }
    }

    public void Write(string file, TimelineSerializationOptions? serializationOptions = null)
    {
        using var fs = File.Create(file);
        Write(fs, serializationOptions);
    }

    public void Write(Stream stream, TimelineSerializationOptions? serializationOptions = null)
    {
        if (Timeline is null)
            throw new InvalidOperationException("Timeline is null. Cannot write to file.");

        serializationOptions ??= new TimelineSerializationOptions();
        if (serializationOptions.CalculateTotalFrameCount)
            Timeline.LastFrameIndex = Timeline.CalculateTimelineFrameLength();
        else
            Timeline.CheckTimelineElementFrameRanges();

        SmartBinaryStream bs = new SmartBinaryStream(stream, stringCoding: StringCoding.ZeroTerminated);

        long basePos = bs.Position;

        // Write Header
        bs.WriteInt32(MAGIC);
        bs.WriteUInt32(Version);
        bs.WritePadding(0x10);
        bs.WriteInt32(0);

        bs.Position = basePos + 0x20;

        // The timeline header can appear after some element data, which is weird, I wonder why they do this.
        // Write said data first.
        Timeline.WritePreTimelineElements(bs);
        long timelineOffset = bs.Position;

        bs.Position = basePos + 0x18;

        long lastDataPos = timelineOffset;
        using (var marker = bs.PushMarker(lastDataPos + 0x24))
        {
            bs.WriteStructPointer(basePos, Timeline, ref lastDataPos);
            bs.Position = marker.LastDataPosition;
        }

        // String table.
        bs.WriteStringTable();

    }
}

public class TimelineSerializationOptions
{
    /// <summary>
    /// Whether to calculate the number of frames based on the current elements in a <see cref="Timeline"/>. Defaults to true.
    /// </summary>
    public bool CalculateTotalFrameCount { get; set; } = true;
}