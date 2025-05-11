using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

using FF16Tools.Files.Nex.Managers;

namespace FF16Tools.Files.Nex;

public class NexDataFile
{
    /// <summary>
    /// Magic, 'NXDF'.
    /// </summary>
    public const uint MAGIC = 0x4644584E;

    public byte[]? Buffer { get; set; }

    public uint Version { get; set; }
    public NexTableType Type { get; set; }
    public NexTableCategory Category { get; set; }
    public bool UsesBaseRowId { get; set; }
    public uint BaseRowId { get; set; }

    /// <summary>
    /// Row manager. Allows access to underlying row information.
    /// </summary>
    public INexRowManager? RowManager { get; set; }

    public static NexDataFile FromFile(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        NexDataFile nxdFile = new NexDataFile();
        nxdFile.Read(File.ReadAllBytes(file));
        return nxdFile;
    }

    public void Read(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        Buffer = data;

        var ms = new MemoryStream(Buffer);
        var bs = new BinaryStream(ms);
        if (bs.ReadUInt32() != MAGIC)
            throw new InvalidDataException("Not a Nex Data file.");

        Version = bs.ReadUInt32();
        Type = (NexTableType)bs.Read1Byte();
        Category = (NexTableCategory)bs.Read1Byte();
        UsesBaseRowId = bs.ReadBoolean();
        bs.Read1Byte();
        BaseRowId = bs.ReadUInt32();
        bs.Position += (4 * sizeof(uint));

        switch (Category)
        {
            case NexTableCategory.SingleKeyed:
            case NexTableCategory.SingleKeyed_Localized:
                RowManager = new NexRowTableManager();
                break;

            case NexTableCategory.DoubleKeyed:
            case NexTableCategory.DoubleKeyed_Localized:
                RowManager = new NexDoubleKeyedRowTableManager();
                break;

            case NexTableCategory.TripleKeyed:
            case NexTableCategory.TripleKeyed_Localized:
                RowManager = new NexTripleKeyedRowTableManager();
                break;

            default:
                throw new NotSupportedException($"Table category type '{Category}' is not supported");
        }

        RowManager.Read(bs);
    }
}