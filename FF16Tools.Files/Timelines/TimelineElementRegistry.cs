using FF16Tools.Files.Timelines.Elements.Battle;
using FF16Tools.Files.Timelines.Elements.General;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines;

public class TimelineElementRegistry
{
    /// <summary>
    /// List of all supported timeline elements.
    /// </summary>
    public static readonly Dictionary<TimelineElementType, TimelineElementBase> ElementList = new()
    {
        [TimelineElementType.kTimelineElem_5] = new TimelineElement_5(),
        [TimelineElementType.CameraAnimationRange] = new CameraAnimationRange(),
        [TimelineElementType.kTimelineElem_9] = new TimelineElement_9(),
        [TimelineElementType.BattleCondition] = new BattleCondition(),
        [TimelineElementType.BulletTimeRange] = new BulletTimeRange(),
        [TimelineElementType.ControlPermission] = new ControlPermission(),
        [TimelineElementType.kTimelineElem_23] = new TimelineElement_23(),
        [TimelineElementType.kTimelineElem_26] = new TimelineElement_26(),
        [TimelineElementType.AdjustRootMoveRange] = new AdjustRootMoveRange(),
        [TimelineElementType.kTimelineElem_30] = new TimelineElement_30(),
        [TimelineElementType.PlaySoundTrigger] = new PlaySoundTrigger(),
        [TimelineElementType.AttachWeaponTemporaryRange] = new AttachWeaponTemporaryRange(),
        [TimelineElementType.ModelSE] = new ModelSE(),
        [TimelineElementType.BattleMessageRange] = new BattleMessageRange(),
        [TimelineElementType.kTimelineElem_49] = new TimelineElement_49(),
        [TimelineElementType.EnableDestructorCollision] = new EnableDestructorCollision(),
        [TimelineElementType.PadVibrationUnk] = new PadVibrationUnk(),
        [TimelineElementType.kTimelineElem_60] = new TimelineElement_60(),
        [TimelineElementType.kTimelineElem_73] = new TimelineElement_73(),
        [TimelineElementType.kTimelineElem_84] = new TimelineElement_84(),

        [TimelineElementType.kTimelineElem_1001] = new TimelineElement_1001(),
        [TimelineElementType.Attack] = new Attack(),
        [TimelineElementType.kTimelineElem_1004] = new TimelineElement_1004(),
        [TimelineElementType.kTimelineElem_1005] = new TimelineElement_1005(),
        [TimelineElementType.kTimelineElem_1007] = new TimelineElement_1007(),
        [TimelineElementType.kTimelineElem_1008] = new TimelineElement_1008(),
        [TimelineElementType.TurnToTarget] = new TurnToTarget(),
        [TimelineElementType.kTimelineElem_1014] = new TimelineElement_1014(),
        [TimelineElementType.PrecedeInputUnk] = new TimelineElement_1016(),
        [TimelineElementType.kTimelineElem_1023] = new TimelineElement_1023(),
        [TimelineElementType.kTimelineElem_1030] = new TimelineElement_1030(),
        [TimelineElementType.kTimelineElem_1035] = new TimelineElement_1035(),
        [TimelineElementType.SummonPartsVisibleRange] = new SummonPartsVisibleRange(),
        [TimelineElementType.kTimelineElem_1049] = new TimelineElement_1049(),
        [TimelineElementType.BattleVoiceTrigger] = new BattleVoiceTrigger(),
        [TimelineElementType.DisableReceiver] = new DisableReceiver(),
        [TimelineElementType.kTimelineElem_1059] = new TimelineElement_1059(),
        [TimelineElementType.kTimelineElem_1064] = new TimelineElement_1064(),
        [TimelineElementType.MotionAttribute] = new MotionAttribute(),
        [TimelineElementType.kTimelineElem_1075] = new TimelineElement_1075(),
        [TimelineElementType.kTimelineElem_1084] = new TimelineElement_1084(),
        [TimelineElementType.DisableCharaUnk] = new TimelineElement_1097(),
        [TimelineElementType.kTimelineElem_1099] = new TimelineElement_1099(),
        [TimelineElementType.kTimelineElem_1102] = new TimelineElement_1102(),
        [TimelineElementType.kTimelineElem_1103] = new TimelineElement_1103(),
        // TimelineElementType.kTimelineElem_1107 => throw new NotSupportedException($"Timeline element {type} not yet supported"),
        //TimelineElementType.ExtrudeLayerRange => new ExtrudeLayerRange(),
        [TimelineElementType.kTimelineElem_1115] = new TimelineElement_1115(),
        [TimelineElementType.StartCooldown] = new StartCooldown(),
        [TimelineElementType.JustBuddyCommand] = new JustBuddyCommand(),
    };
}
