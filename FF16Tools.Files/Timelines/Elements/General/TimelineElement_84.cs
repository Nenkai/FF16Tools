using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.General;

public class TimelineElement_84 : TimelineElementBase, ISerializableStruct
{
    public CameraAnimationRange.Sub8Struct[] Entries { get; set; } = new CameraAnimationRange.Sub8Struct[9];
    public int Unk { get; set; }
    public int[] Unks { get; set; } = new int[4];

    public TimelineElement_84()
    {
        UnionType = TimelineElementType.kTimelineElem_84;
        for (int i = 0; i < 9; i++)
            Entries[i] = new CameraAnimationRange.Sub8Struct();
    }

    public override void Read(SmartBinaryStream bs)
    {
        for (int i = 0; i < 9; i++)
        {
            Entries[i] = new CameraAnimationRange.Sub8Struct();
            Entries[i].Read(bs);
        }

        Unk = bs.ReadInt32();

        for (int i = 0; i < 4; i++)
            Unks[i] = bs.ReadInt32();
    }

    public override void Write(SmartBinaryStream bs)
    {
        throw new NotImplementedException();

        for (int i = 0; i < 9; i++)
            Entries[i].Write(bs);

        Unk = bs.ReadInt32();

        for (int i = 0; i < 4; i++)
            bs.WriteInt32(Unks[i]);
    }

    public uint GetSize()
    {
        uint size = GetMetaSize() +
            (9 * 0x10) +
            0x04 +
            (4 * sizeof(int));

        return size;
    }
}
