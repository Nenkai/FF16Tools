using System.Buffers.Binary;

namespace FF16Tools.Files.Timelines.Chara;

public class CharaTimelineIdsFile
{
    /// <summary>
    /// 'idlt'
    /// </summary>
    public static uint MAGIC => BinaryPrimitives.ReadUInt32LittleEndian("idlt"u8);
    private const int PathStringBufferSize = 0x80;

    public Dictionary<uint, string> TimelineIdsToFiles { get; private set; } = [];

    public static CharaTimelineIdsFile Open(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        using var fs = File.OpenRead(file);
        var charaFile = new CharaTimelineIdsFile();
        charaFile.Read(fs);
        return charaFile;
    }

    public void Read(string file)
    {
        using var fs = File.OpenRead(file);
        Read(fs);
    }

    public void Read(Stream stream)
    {
        var bs = new SmartBinaryStream(stream);

        if (bs.ReadUInt32() != MAGIC)
            throw new InvalidDataException("Not a chara timeline ids (.idl) file. Magic did not match.");

        uint numFiles = bs.ReadUInt32();
        TimelineIdsToFiles = [];
        for (int i = 0; i < numFiles; i++)
        {
            uint id = bs.ReadUInt32();
            string fileName = bs.ReadStringFix(PathStringBufferSize);
            TimelineIdsToFiles.Add(id, fileName);
        }
    }

    public void Write(string file)
    {
        using var fs = File.Create(file);
        Write(fs);
    }

    public void Write(Stream stream)
    {
        var bs = new SmartBinaryStream(stream);

        bs.WriteUInt32(MAGIC);
        bs.WriteUInt32((uint)TimelineIdsToFiles.Count);

        foreach (var kvp in TimelineIdsToFiles)
        {
            bs.WriteUInt32(kvp.Key);
            bs.WriteStringFix(kvp.Value, PathStringBufferSize);
        }
    }

    public void MergeWith(CharaTimelineIdsFile other, bool overwriteExisting = false)
    {
        MergeWith(other.TimelineIdsToFiles, overwriteExisting);
    }

    public void MergeWith(Dictionary<uint, string> idsToFiles, bool overwriteExisting = false)
    {
        foreach (var kvp in idsToFiles)
        {
            if (overwriteExisting)
            {
                TimelineIdsToFiles[kvp.Key] = kvp.Value;
            }
            else
            {
                string currentValue = TimelineIdsToFiles.GetValueOrDefault(kvp.Key, kvp.Value);
                if (kvp.Value != currentValue)
                    throw new InvalidOperationException($"Cannot merge id lists as they contain conflicting values.");
                else
                {
                    TimelineIdsToFiles.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
