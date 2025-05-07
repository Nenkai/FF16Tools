using Syroot.BinaryData;

namespace FF16Tools.Files.CharaTimeline;

public class CharaTimelineFile
{
    public const int magic = 0x4C544346;
    public uint Version { get; set; }
    public Timeline Timeline { get; set; }

    public void Read(string file)
    {
        using var fs = File.OpenRead(file);
        Read(fs);
    }

    public void Read(Stream stream)
    {
        var bs = new BinaryStream(stream);

        if (bs.ReadUInt32() != magic)
            throw new InvalidDataException("Not a chara timeline (.tlb) file. Magic did not match.");

        Version = bs.ReadUInt32();
        bs.Position += 0x10;
        int timelineDataPosition = bs.ReadInt32();

        bs.Position = timelineDataPosition;
        Timeline = new Timeline();
        Timeline.Read(bs);
    }

    public void Write(string file)
    {
        if (Timeline == null)
            throw new InvalidOperationException("Timeline is null. Cannot write to file.");

        using var buffer = new MemoryStream();
        using BinaryStream bs = new BinaryStream(buffer);

        ///* The file structure is as follows:
        // * Header
        // * Arrays of structs referenced by timeline elements, and specific element Data Unions
        // * Timeline
        // *  TimelineElementsList[]
        // *      Elements
        // *  DataUnion (referenced in Elements)
        // *      ElementData        
        // *  AssetGroups[]
        // *      AssetGroup
        // *  AssetOffsetsList[] (referenced in AssetGroup)
        // *  Asset[] (referenced from AssetOffsetsList)
        // *  FinalStructsArray[] // for a lack of a better name, its just the struct that appears at the end of the file
        // *      FinalStruct
        // *          SubStruct
        // *  All distinct strings referenced everywhere 
        // */

        // Write Header
        bs.WriteInt32(magic);
        bs.WriteUInt32(Version + 1);
        bs.WriteInt32(0); // c
        bs.WriteInt32(0); // d
        bs.WriteInt32(0); // e
        bs.WriteInt32(0); // f

        // The final field of the header is the position of the timeline, which the timeline will write in its own write method
        Timeline.Write(bs);

        buffer.Position = 0;
        using (var fileStream = File.Create(file))
        {
            buffer.CopyTo(fileStream);
        }
    }
}
