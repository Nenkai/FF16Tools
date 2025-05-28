using FF16Tools.Files.Timelines.Chara;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Elements.Battle;

/// <summary>
/// Triggers an event for the current action. <br/>
/// (Name is guessed)
/// </summary>
public class ActionEventTrigger : TimelineElementBase, ITimelineTriggerElement
{
    public ActionEventTrigger()
    {
        UnionType = TimelineElementType.ActionEventTrigger;
    }

    /// <summary>
    /// Some kind of Id (1-88), for example 1/2 spawn the satelite skill, 11 starts skill cooldown
    /// </summary>
    public int EventId { get; set; }

    public override void Read(SmartBinaryStream bs)
    {
        ReadMeta(bs);

        EventId = bs.ReadInt32();
        bs.ReadCheckPadding(0x20);
    }

    public override void Write(SmartBinaryStream bs)
    {
        WriteMeta(bs);

        bs.WriteInt32(EventId);
        bs.WritePadding(0x20);
    }

    public override uint GetSize() => GetMetaSize() + 0x24;
}

