using FF16Tools.Files.Panzer;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using YamlDotNet.Serialization.ObjectFactories;

namespace FF16Tools.Files.Magic;

public class MagicFile
{
    public const ushort VERSION_LATEST = 5;

    public Dictionary<uint, MagicEntry> MagicEntries { get; set; } = [];

    public static MagicFile Open(string path)
    {
        using var fs = File.OpenRead(path);
        var file = new MagicFile();
        file.Read(fs);
        return file;
    }

    public void Read(Stream stream)
    {
        using var bs = new SmartBinaryStream(stream);
        long basePos = bs.Position;

        uint version = bs.ReadUInt32();
        if (version != VERSION_LATEST)
            throw new NotSupportedException("Only Panzer files version 2 (FF16) is currently supported.");

        uint numSections = bs.ReadUInt32();

        long sectionsOffset = bs.Position;
        List<MagicFileSection> sections = new List<MagicFileSection>((int)numSections);
        for (int i = 0; i < numSections; i++)
        {
            var section = new MagicFileSection
            {
                Index = bs.ReadInt32(),
                Offset = bs.ReadInt32(),
                Size = bs.ReadInt32()
            };
            sections.Add(section);
        }

        Debug.Assert(sections.Count > 0, "Missing main section in magic file?");
        bs.Position = sectionsOffset + sections[0].Offset;
        ReadMagicSection(bs);
    }

    public void Write(Stream stream)
    {
        long basePos = stream.Position;

        var bs = new SmartBinaryStream(stream, stringCoding: Syroot.BinaryData.StringCoding.ZeroTerminated);
        bs.WriteUInt32(VERSION_LATEST);
        bs.WriteUInt32(2); // Always 2 sections

        long sectionsOffset = bs.Position;
        bs.Position += (2 * 0x0C); // Skip sections for now

        long magicEntriesOffset = bs.Position;
        bs.WriteUInt32((uint)MagicEntries.Count);

        long lastSection1DataOffset = bs.Position + (new MagicEntry().GetSize() * MagicEntries.Count);
        foreach (var entry in MagicEntries.Values)
        {
            using var marker = bs.PushMarker(lastSection1DataOffset);
            entry.Write(bs);

            lastSection1DataOffset = marker.LastDataPosition;
        }

        bs.Position = lastSection1DataOffset;

        // XXX: Section info serialization could be improved. The game really only uses Section[0] directly so we aren't too worried about it for now.
        // Write section 2 data (it's just 4 empty bytes)
        long section2Offset = bs.Position;
        bs.WriteUInt32(0); // 
        long lastSection2DataOffset = bs.Position;

        // Write first section
        bs.Position = basePos + 0x08;
        bs.WriteUInt32(0); // Index
        bs.WriteUInt32((uint)(magicEntriesOffset - (sectionsOffset + (0 * 0x0C))));
        bs.WriteUInt32((uint)(lastSection1DataOffset - magicEntriesOffset)); // Size

        // Write second section
        bs.WriteUInt32(1); // Index
        bs.WriteUInt32((uint)(section2Offset - (sectionsOffset + (1 * 0x0C))));
        bs.WriteUInt32((uint)(lastSection2DataOffset - section2Offset)); // Size

        bs.Position = lastSection2DataOffset;
    }

    private void ReadMagicSection(SmartBinaryStream bs)
    {
        uint numMagicEntries = bs.ReadUInt32();
        bs.ReadStructArrayWithCallback<MagicEntry>(numMagicEntries, (elem) =>
        {
            MagicEntries.Add(elem.Id, elem);
        });
    }
}

public record struct MagicFileSection
{
    public int Index { get; set; }
    public int Offset { get; set; }
    public int Size { get; set; }
}
