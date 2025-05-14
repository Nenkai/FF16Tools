using FF16Tools.Files.Timelines.Elements;
using FF16Tools.Files.Timelines.Elements.Battle;
using FF16Tools.Files.Timelines.Elements.General;

using Syroot.BinaryData;

namespace FF16Tools.Files.Timelines.Chara;

public class TimelineElement : ISerializableStruct
{
    public uint Field_0x00 { get; set; }
    public string? Name { get; set; }
    public uint TimelineElemUnionTypeOrLayerId { get; set; }
    public uint FrameStart { get; set; }
    public uint NumFrames { get; set; }
    public uint Field_0x14 { get; set; }
    public byte Field_0x18 { get; set; }
    public byte Field_0x19 { get; set; }
    public byte Field_0x1A { get; set; }
    public byte Field_0x1B { get; set; }
    public TimelineElementBase? DataUnion { get; set; }

    public void Read(SmartBinaryStream bs)
    {
        long thisPos = bs.Position;

        Field_0x00 = bs.ReadUInt32();
        Name = bs.ReadStringPointer(thisPos);
        TimelineElemUnionTypeOrLayerId = bs.ReadUInt32();
        FrameStart = bs.ReadUInt32();
        NumFrames = bs.ReadUInt32();
        Field_0x14 = bs.ReadUInt32();
        Field_0x18 = bs.Read1Byte();
        Field_0x19 = bs.Read1Byte();
        Field_0x1A = bs.Read1Byte();
        Field_0x1B = bs.Read1Byte();
        int dataHdrOffset = bs.ReadInt32();

        bs.Position = thisPos + dataHdrOffset;
        TimelineElementType type = (TimelineElementType)bs.ReadUInt32();
        bs.Position -= 4;

        DataUnion = TimelineElementFactory.CreateElement(type);
        DataUnion?.Read(bs);
    }

    /// <summary>
    /// Note: won't write data, only header.
    /// </summary>
    /// <param name="bs"></param>
    public void Write(SmartBinaryStream bs)
    {
        long thisPos = bs.Position;

        bs.WriteUInt32(Field_0x00);
        bs.AddStringPointer(Name, thisPos);
        bs.WriteUInt32(TimelineElemUnionTypeOrLayerId);
        bs.WriteUInt32(FrameStart);
        bs.WriteUInt32(NumFrames);
        bs.WriteUInt32(Field_0x14);
        bs.WriteByte(Field_0x18);
        bs.WriteByte(Field_0x19);
        bs.WriteByte(Field_0x1A);
        bs.WriteByte(Field_0x1B);
        bs.WriteObjectPointer(DataUnion, thisPos);
    }

    public uint GetSize() => 0x20;
}
