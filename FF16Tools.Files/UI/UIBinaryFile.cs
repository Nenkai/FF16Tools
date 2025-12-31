using FF16Tools.Files.Panzer;
using FF16Tools.Files.UI.Assets;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI;

public class UIBinaryFile
{
    public static readonly uint MAGIC = BinaryPrimitives.ReadUInt32LittleEndian("UIB\0"u8);
    public const ushort VERSION_LATEST = 10;

    public AssetRegistry Assets { get; set; } = new();
    public List<UIComponentInfo> Components { get; set; } = [];

    public static UIBinaryFile Open(string path)
    {
        using var fs = File.OpenRead(path);
        var file = new UIBinaryFile();
        file.Read(fs);
        return file;
    }

    public void Read(Stream stream)
    {
        using var bs = new SmartBinaryStream(stream);
        long basePos = bs.Position;

        uint magic = bs.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException("Not a UI (.uib) file. Magic did not match UIB.");

        uint version = bs.ReadUInt32();
        if (version != VERSION_LATEST)
            throw new NotSupportedException("Only UIB files version 10 (FF16) is currently supported.");

        bs.ReadCheckPadding(0x10);

        uint tocOffset = bs.ReadUInt32();
        bs.ReadCheckPadding(0x10);

        bs.Position = tocOffset;
        ReadToC(bs);
    }

    private void ReadToC(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        Assets = bs.ReadStructPointer<AssetRegistry>(basePos);
        Components = bs.ReadStructArrayFromOffsetCount<UIComponentInfo>(basePos);
        bs.ReadCheckPadding(0x20);
    }

    public void Write(Stream stream)
    {
        long basePos = stream.Position;

        var bs = new SmartBinaryStream(stream, stringCoding: Syroot.BinaryData.StringCoding.ZeroTerminated);
        bs.WriteUInt32(MAGIC);
        bs.WriteUInt32(VERSION_LATEST);
        bs.WritePadding(0x10);

        bs.WriteUInt32(0x2C); // ToC offset
        bs.WritePadding(0x10);

        WriteToC(bs);

        bs.WriteStringTable(writeEmptyFirst: false);
    }

    private void WriteToC(SmartBinaryStream bs)
    {
        long thisStructPos = bs.Position;
        long lastDataOffset = thisStructPos + 0x2C;

        bs.WriteStructPointer(thisStructPos, Assets, ref lastDataOffset);
        bs.WriteStructArrayPointer(thisStructPos, Components, ref lastDataOffset);
        bs.WritePadding(0x20);

        bs.Position = lastDataOffset;
    }
}
