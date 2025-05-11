using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace FF16Tools.Files.VFX;

/// <summary>
/// .vatb files
/// </summary>
public class VFXAudioTableBinary
{
    // Normally this would be a dictionary. But files like vfx/chara/c5010/vfx/elite_debu/ve/c5010_elite_debu.vatb
    // Has an entry that appears twice. [1907] C5010_ELITE_DEBU_TRAIL_SIDE_01U
    public List<VFXAudioTableEntry> Entries { get; set; } = [];

    public static VFXAudioTableBinary Open(string file)
    {
        using var fs = File.OpenRead(file);
        return Open(fs);
    }

    public static VFXAudioTableBinary Open(Stream stream)
    {
        var bin = new VFXAudioTableBinary();
        bin.Read(stream);

        return bin;
    }

    public static VFXAudioTableBinary FromJson(string json)
    {
        var bin = new VFXAudioTableBinary();
        bin.Entries = JsonSerializer.Deserialize<List<VFXAudioTableEntry>>(json) ?? [];
        return bin;
    }

    public void Read(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream, ByteConverter.Little);

        uint headerSize = bs.ReadUInt32();
        uint entryCount = bs.ReadUInt32();

        for (int i = 0; i < entryCount; i++)
        {
            bs.Position = headerSize + (i * 0x20);
            var entry = new VFXAudioTableEntry();
            entry.Read(bs);
            Entries.Add(entry);

        }
    }

    public void Write(Stream outputStream)
    {
        BinaryStream bs = new BinaryStream(outputStream);
        bs.WriteUInt32(0x08);
        bs.WriteUInt32((uint)Entries.Count);

        // Find out where strings should be
        long lastAssetRefOffset = 0x08 + (Entries.Count * VFXAudioTableEntry.GetSize());
        long lastStrOffset = lastAssetRefOffset;
        foreach (VFXAudioTableEntry entry in Entries)
        {
            if (entry.VFX is not null)
                lastStrOffset += 0x08;

            if (entry.Audio is not null)
                lastStrOffset += 0x08;
        }

        int i = 0;
        foreach (VFXAudioTableEntry entry in Entries)
        {
            long entryOffset = 0x08 + (i * VFXAudioTableEntry.GetSize());
            bs.Position = entryOffset;

            bs.WriteUInt32(entry.Id);
            WriteStringPointer(bs, entry.Name ?? string.Empty, entryOffset, ref lastStrOffset);

            // Write asset refs first
            uint vfxRefOffset = 0;
            if (entry.VFX is not null)
            {
                bs.Position = lastAssetRefOffset;
                bs.WriteUInt32(entry.VFX.AssetType);
                WriteStringPointer(bs, entry.VFX.Path ?? string.Empty, lastAssetRefOffset, ref lastStrOffset);
                vfxRefOffset = (uint)(lastAssetRefOffset - entryOffset);

                lastAssetRefOffset += AssetReference.GetSize();
                bs.Position = entryOffset + 0x08;
            }
            bs.WriteUInt32(vfxRefOffset);

            bs.WriteUInt32(0);
            bs.WriteUInt32(0);
            bs.WriteUInt32(0);

            uint audioRefOffset = 0;
            if (entry.Audio is not null)
            {
                bs.Position = lastAssetRefOffset;
                bs.WriteUInt32(entry.Audio.AssetType);
                WriteStringPointer(bs, entry.Audio.Path ?? string.Empty, lastAssetRefOffset, ref lastStrOffset);
                audioRefOffset = (uint)(lastAssetRefOffset - entryOffset);

                lastAssetRefOffset += AssetReference.GetSize();
                bs.Position = entryOffset + 0x18;
            }
            bs.WriteUInt32(audioRefOffset);

            bs.WriteUInt32(0);
            i++;
        }

        
    }

    private void WriteStringPointer(BinaryStream bs, string name, long relativeOffset, ref long lastOffset)
    {
        long cPos = bs.Position;
        int thisStrOffset = (int)(lastOffset - relativeOffset);
        bs.WriteInt32(thisStrOffset);

        bs.Position = lastOffset;
        bs.WriteString(name, StringCoding.ZeroTerminated);
        lastOffset = bs.Position;

        bs.Position = cPos + 0x04;
    }

    public string ToJson(JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(Entries, options: options ?? new JsonSerializerOptions() {  WriteIndented = true });
    }
}

public class VFXAudioTableEntry
{
    public uint Id { get; set; }
    public string? Name { get; set; }
    public ulong Flags { get; set; }
    public AssetReference VFX { get; set; } = new();
    public AssetReference Audio { get; set; } = new();

    public void Read(BinaryStream bs)
    {
        long basePos = bs.Position;

        Id = bs.ReadUInt32();
        uint nameOffset = bs.ReadUInt32();
        uint vfxFileOffset = bs.ReadUInt32();
        uint field_0x0C = bs.ReadUInt32();
        Debug.Assert(field_0x0C == 0x00);

        Flags = bs.ReadUInt64();

        uint audioFileOffset = bs.ReadUInt32();
        uint field_0x1C = bs.ReadUInt32();
        Debug.Assert(field_0x1C == 0x00);

        bs.Position = basePos + nameOffset;
        Name = bs.ReadString(StringCoding.ZeroTerminated);

        bs.Position = basePos + vfxFileOffset;
        VFX.Read(bs);

        bs.Position = basePos + audioFileOffset;
        Audio.Read(bs);
    }

    public static uint GetSize()
    {
        return 0x20;
    }
}

public class AssetReference
{
    // 1012 = Audio (.sab)
    // 1019 = VFX (.vfx)
    public uint AssetType { get; set; }
    public string? Path { get; set; }

    public void Read(BinaryStream bs)
    {
        long basePos = bs.Position;

        AssetType = bs.ReadUInt32();
        uint pathOffset = bs.ReadUInt32();

        bs.Position = basePos + pathOffset;
        Path = bs.ReadString(StringCoding.ZeroTerminated);
    }

    public static uint GetSize()
    {
        return 0x08;
    }
}