using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FF16Tools.Files.Nex.Entities;

using Syroot.BinaryData;

namespace FF16Tools.Files.Panzer;

/// <summary>
/// DevEnv.Panzer.Models.Binary.Pzd.PzdTextContent
/// </summary>
public class PzdTextContent : ISerializableStruct
{
    public uint Id { get; set; }
    public string? Line { get; set; }

    /// <summary>
    /// Valid is speaker, bnpcbase, enpcbase.
    /// </summary>
    public NexUnionKey Speaker { get; set; }

    public string? VoiceSoundPath { get; set; }
    public LineShowType ShowType { get; set; }
    public string? ShortVoiceSoundPath { get; set; }
    public bool IsShortened { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        long basePos = bs.Position;
        Id = bs.ReadUInt32();
        Line = bs.ReadStringPointer(basePos);
        Speaker = NexUnionKey.FromStream(bs);
        VoiceSoundPath = bs.ReadStringPointer(basePos);
        ShowType = (LineShowType)bs.ReadInt32();
        ShortVoiceSoundPath = bs.ReadStringPointer(basePos);
        IsShortened = bs.ReadBoolean();
        bs.ReadCheckPadding(3);
    }


    public void Write(SmartBinaryStream bs)
    {
        long basePos = bs.Position;

        bs.WriteUInt32(Id);
        bs.AddStringPointer(Line, basePos);
        Speaker.Write32(bs);
        bs.AddStringPointer(VoiceSoundPath, basePos);
        bs.WriteUInt32((uint)ShowType);
        bs.AddStringPointer(ShortVoiceSoundPath, basePos);
        bs.WriteBoolean(IsShortened);
        bs.WritePadding(3);
    }

    public uint GetSize() => 0x20;

    public override string ToString()
    {
        return $"[{Id}] {Speaker} - {Line}";
    }
}

public enum LineShowType
{
    /// <summary>
    /// This subtitle will show unless the player has disabled subtitles.
    /// </summary>
    Normal,

    /// <summary>
    /// Always shows regardless of subtitle settings.
    /// </summary>
    AlwaysShow,

    /// <summary>
    /// Hearing impaired subtitle. It will only show if "Hearing Impaired Subtitle" is turned on.
    /// </summary>
    HearingImpairedSubtitle
}
