using Syroot.BinaryData;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FF16Tools.Files.Panzer;

/// <summary>
/// PanzerDataFile - DevEnv.Panzer.Models.Binary.Pzd.PzdFile
/// </summary>
public class PzdFile
{
    public static readonly uint MAGIC = BinaryPrimitives.ReadUInt32LittleEndian("PZDF"u8);
    public const ushort VERSION_LATEST = 2;

    public Dictionary<uint, PzdTextContent> Lines { get; private set; } = [];

    public static PzdFile Open(string path)
    {
        using var fs = File.OpenRead(path);
        var file = new PzdFile();
        file.Read(fs);
        return file;
    }

    public void Read(Stream stream)
    {
        using var bs = new SmartBinaryStream(stream);
        long basePos = bs.Position;

        // Read DevEnv.Panzer.Models.Binary.Pzd.PzdFileHeader
        uint magic = bs.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException("Not a Panzer (.pzd) file. Magic did not match PZDF.");

        ushort version = bs.ReadUInt16();
        if (version != VERSION_LATEST)
            throw new NotSupportedException("Only Panzer files version 2 (FF16) is currently supported.");

        bs.Position = 0x20;
        bs.ReadStructArrayFromOffsetCountWithCallback<PzdTextContent>(basePos, (elem) => Lines.Add(elem.Id, elem));
    }

    public void Write(Stream stream)
    {
        long basePos = stream.Position;

        var bs = new SmartBinaryStream(stream, stringCoding: StringCoding.ZeroTerminated);
        bs.WriteUInt32(MAGIC);
        bs.WriteUInt16(VERSION_LATEST); // Version

        // Skip header, go straight to data for now.
        bs.Position = basePos + 0x30;
        bs.AddObjectPointer(Lines);
        bs.WriteStructArray(Lines.Values.ToList());
        bs.WriteStringTable();

        long endPos = bs.Position;

        // Finish header.
        bs.Position = basePos + 0x20;
        bs.WriteObjectPointer(Lines, basePos); // Offset to lines
        bs.WriteUInt32((uint)Lines.Count);

        // We don't need the BVLD header.

        bs.Position = endPos;
    }

    public void ReadFromYaml(TextReader reader)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        List<PzdTextContent> list = deserializer.Deserialize<List<PzdTextContent>>(reader);

        foreach (var ent in list)
            Lines.Add(ent.Id, ent);
    }

    public void WriteAsYaml(TextWriter writer)
    { 
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .DisableAliases()
            .Build();
        serializer.Serialize(writer, Lines.Select(e => e.Value).ToList());
    }

    public void WriteAsXml(TextWriter writer)
    {
        using var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings()
        {
            Indent = true,
        });
        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("Panzer");

        var lineSerializer = new XmlSerializer(typeof(PzdTextContent));
        foreach (KeyValuePair<uint, PzdTextContent> lineKv in Lines)
        {
            lineSerializer.Serialize(writer, lineKv.Value);
        }
        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndDocument();
    }
}
