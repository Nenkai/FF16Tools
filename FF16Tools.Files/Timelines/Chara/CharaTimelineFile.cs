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
        bs.Position += 0x10;
        int timelineDataPosition = bs.ReadInt32();

        if (timelineDataPosition != 0)
        {
            bs.Position = timelineDataPosition;
            Timeline = new Timeline();
            Timeline.Read(bs);
        }
    }

    public void Write(string file)
    {
        using var fs = File.Create(file);
        Write(fs);
    }

    public void Write(Stream stream, CharaTimelineSerializationOptions? serializationOptions = null)
    {
        if (Timeline is null)
            throw new InvalidOperationException("Timeline is null. Cannot write to file.");

        serializationOptions ??= new CharaTimelineSerializationOptions();

        SmartBinaryStream bs = new SmartBinaryStream(stream, stringCoding: StringCoding.ZeroTerminated);

        long basePos = bs.Position;

        // Write Header
        bs.WriteInt32(MAGIC);
        bs.WriteUInt32(Version);
        bs.WritePadding(0x10);
        bs.WriteInt32(0); // Timeline offset, write later

        // Write the timeline
        int timelineHeaderOffset = Timeline.Write(bs, serializationOptions);

        long tempPos = bs.Position;

        // Write the timeline offset
        bs.Position = basePos + 0x18;
        bs.WriteInt32(timelineHeaderOffset);

        bs.Position = tempPos;
    }
}

public class CharaTimelineSerializationOptions
{
    /// <summary>
    /// Whether to calculate the number of frames based on the current elements in a <see cref="Timeline"/>. Defaults to true.
    /// </summary>
    public bool CalculateTotalFrameCount { get; set; } = true;
}