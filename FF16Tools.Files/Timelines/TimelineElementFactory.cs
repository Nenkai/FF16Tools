using FF16Tools.Files.Timelines.Elements.Battle;
using FF16Tools.Files.Timelines.Elements.General;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines;

public class TimelineElementFactory
{
    public static TimelineElementBase CreateElement(TimelineElementType type)
    {
        return type switch
        {
            TimelineElementType.kTimelineElem_5 => new TimelineElement_5(),
            TimelineElementType.CameraAnimationRange => new CameraAnimationRange(),
            TimelineElementType.kTimelineElem_9 => new TimelineElement_9(),
            TimelineElementType.BattleCondition => new BattleCondition(),
            TimelineElementType.kTimelineElem_11 => new TimelineElement_11(),
            TimelineElementType.BulletTimeRange => new BulletTimeRange(),
            TimelineElementType.ControlPermission => new ControlPermission(),
            TimelineElementType.kTimelineElem_23 => new TimelineElement_23(),
            TimelineElementType.kTimelineElem_24 => new TimelineElement_24(),
            TimelineElementType.kTimelineElem_26 => new TimelineElement_26(),
            TimelineElementType.AdjustRootMoveRange => new AdjustRootMoveRange(),
            TimelineElementType.kTimelineElem_30 => new TimelineElement_30(),
            TimelineElementType.PlaySoundTrigger => new PlaySoundTrigger(),
            TimelineElementType.AttachWeaponTemporaryRange => new AttachWeaponTemporaryRange(),
            TimelineElementType.ModelSE => new ModelSE(),
            TimelineElementType.BattleMessageRange => new BattleMessageRange(),
            TimelineElementType.kTimelineElem_49 => new TimelineElement_49(),
            TimelineElementType.EnableDestructorCollision => new EnableDestructorCollision(),
            TimelineElementType.PadVibrationUnk => new PadVibrationUnk(),
            TimelineElementType.PadVibration => new PadVibration(),
            TimelineElementType.kTimelineElem_60 => new TimelineElement_60(),
            TimelineElementType.kTimelineElem_73 => new TimelineElement_73(),
            TimelineElementType.ControlRejectionRange => new ControlRejectionRange(),
            TimelineElementType.kTimelineElem_84 => new TimelineElement_84(),

            TimelineElementType.kTimelineElem_1001 => new TimelineElement_1001(),
            TimelineElementType.Attack => new Attack(),
            TimelineElementType.kTimelineElem_1004 => new TimelineElement_1004(),
            TimelineElementType.kTimelineElem_1005 => new TimelineElement_1005(),
            TimelineElementType.kTimelineElem_1007 => new TimelineElement_1007(),
            TimelineElementType.kTimelineElem_1008 => new TimelineElement_1008(),
            TimelineElementType.TurnToTarget => new TurnToTarget(),
            TimelineElementType.kTimelineElem_1014 => new TimelineElement_1014(),
            TimelineElementType.PrecedeInputUnk => new TimelineElement_1016(),
            TimelineElementType.kTimelineElem_1021 => new TimelineElement_1021(),
            TimelineElementType.kTimelineElem_1023 => new TimelineElement_1023(),
            TimelineElementType.kTimelineElem_1030 => new TimelineElement_1030(),
            TimelineElementType.FacialPose => new FacialPose(),
            TimelineElementType.SummonPartsVisibleRange => new SummonPartsVisibleRange(),
            TimelineElementType.kTimelineElem_1049 => new TimelineElement_1049(),
            TimelineElementType.kTimelineElem_1050 => new TimelineElement_1050(),
            TimelineElementType.kTimelineElem_1056 => new TimelineElement_1056(),
            TimelineElementType.BattleVoiceTrigger => new BattleVoiceTrigger(),
            TimelineElementType.DisableReceiver => new DisableReceiver(),
            TimelineElementType.kTimelineElem_1059 => new TimelineElement_1059(),
            TimelineElementType.kTimelineElem_1064 => new TimelineElement_1064(),
            TimelineElementType.MotionAttribute => new MotionAttribute(),
            TimelineElementType.kTimelineElem_1068 => new TimelineElement_1068(),
            TimelineElementType.kTimelineElem_1075 => new TimelineElement_1075(),
            TimelineElementType.kTimelineElem_1083 => new TimelineElement_1083(),
            TimelineElementType.kTimelineElem_1084 => new TimelineElement_1084(),
            TimelineElementType.kTimelineElem_1088 => new TimelineElement_1088(),
            TimelineElementType.DisableCharaUnk => new TimelineElement_1097(),
            TimelineElementType.kTimelineElem_1099 => new TimelineElement_1099(),
            TimelineElementType.kTimelineElem_1102 => new TimelineElement_1102(),
            TimelineElementType.kTimelineElem_1103 => new TimelineElement_1103(),
            TimelineElementType.ExtrudeLayerRange => new ExtrudeLayerRange(),
            TimelineElementType.kTimelineElem_1115 => new TimelineElement_1115(),
            TimelineElementType.kTimelineElem_1117 => new TimelineElement_1117(),
            TimelineElementType.kTimelineElem_1123 => new TimelineElement_1123(),
            TimelineElementType.kTimelineElem_1125 => new TimelineElement_1125(),
            TimelineElementType.JustBuddyCommand => new JustBuddyCommand(),
            _ => throw new NotSupportedException($"Timeline element {type} not yet supported"),
        };
    }
}
