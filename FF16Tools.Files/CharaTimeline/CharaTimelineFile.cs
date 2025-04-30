using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline;

public class CharaTimelineFile
{
    public const int magic = 0x4C544346;
    public uint Version { get; set; }
    public Timeline Timeline { get; set; } = new();

    public void Read(string file)
    {
        using var fs = File.OpenRead(file);
        Read(fs);
    }

    public void Read(Stream stream)
    {
        var bs = new BinaryStream(stream);

        if (bs.ReadUInt32() != 0x4C544346)
            throw new InvalidDataException("Not a chara timeline (.tlb) file. Magic did not match.");

        Version = bs.ReadUInt32();
        bs.Position += 0x10;
        int timelineDataPosition = bs.ReadInt32();

        bs.Position = timelineDataPosition;
        Timeline.Read(bs);
    }
}
