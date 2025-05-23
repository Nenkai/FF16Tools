namespace FF16Tools.Files.Timelines;

public enum TimelineElementType
{
    kTimelineElem_5 = 5,
    CameraAnimationRange = 8,
    kTimelineElem_9 = 9,
    BattleCondition = 10,
    kTimelineElem_11 = 11,
    BulletTimeRange = 12,
    ControlPermission = 17,
    kTimelineElem_23 = 23,
    kTimelineElem_24 = 24,
    kTimelineElem_26 = 26,
    AdjustRootMoveRange = 27,
    kTimelineElem_30 = 30,
    PlaySoundTrigger = 31,
    AttachWeaponTemporaryRange = 33,
    ModelSE = 45,
    BattleMessageRange = 47,
    kTimelineElem_49 = 49,
    EnableDestructorCollision = 51,

    /// <summary>
    /// (Not the actual offical name) - does something with vibration, but known
    /// </summary>
    PadVibrationUnk = 56,
    PadVibration = 57,
    kTimelineElem_60 = 60,
    kTimelineElem_73 = 73,
    ControlRejectionRange = 74,
    kTimelineElem_84 = 84,

    //--------------------------------------------------------------
    kTimelineElem_1001 = 1001, // Possibly "motion"
    Attack = 1002,
    kTimelineElem_1004 = 1004,
    kTimelineElem_1005 = 1005,
    kTimelineElem_1007 = 1007,
    kTimelineElem_1008 = 1008,
    ComboEnable = 1009,
    TurnToTarget = 1010,
    MagicCreate = 1012,
    kTimelineElem_1014 = 1014,
    PrecedeInputUnk = 1016,
    kTimelineElem_1021 = 1021,
    kTimelineElem_1023 = 1023,
    kTimelineElem_1030 = 1030,
    FacialPose = 1035,
    SummonPartsVisibleRange = 1047,
    kTimelineElem_1049 = 1049,
    kTimelineElem_1050 = 1050,
    BattleVoiceTrigger = 1053,
    kTimelineElem_1056 = 1056,
    DisableReceiver = 1058,
    kTimelineElem_1059 = 1059,
    kTimelineElem_1064 = 1064,
    MotionAttribute = 1066,
    kTimelineElem_1068 = 1068,
    kTimelineElem_1075 = 1075,
    kTimelineElem_1083 = 1083,
    kTimelineElem_1084 = 1084,
    kTimelineElem_1088 = 1088,
    DisableCharaUnk = 1097,
    kTimelineElem_1099 = 1099,
    kTimelineElem_1102 = 1102,
    kTimelineElem_1103 = 1103,
    kTimelineElem_1107 = 1107,
    ExtrudeLayerRange = 1108, // Actual data layout is unknown
    kTimelineElem_1115 = 1115,
    kTimelineElem_1117 = 1117, // Maybe PlayActionTrigger or EventActionTrigger?
    kTimelineElem_1123 = 1123,
    kTimelineElem_1125 = 1125, // Actual data layout is unknown
    JustBuddyCommand = 1130
}

/* Rest of the unknown types that appear in charatimelines :
 * [1, 4, 6, 7, 9, 11, 13, 18, 19, 20, 21, 22, 24, 28, 29, 34, 35, 36, 37, 38, 42, 43, 46, 48, 50, 52, 53, 55, 59, 62, 63, 67, 70, 
 * 71, 72, 78, 80, 81, 82, 83, 85, 87, 1003, 1006, 1011, 1013, 1015, 1017, 1019, 1020, 1021, 1024, 1025, 1026, 1029, 1032, 1033, 
 * 1034, 1036, 1037, 1038, 1041, 1042, 1043, 1046, 1050, 1051, 1055, 1057, 1061, 1062, 1067, 1068, 1070, 1071, 1072, 1074, 1078, 
 * 1079, 1080, 1081, 1082, 1085, 1086, 1087, 1089, 1091, 1093, 1094, 1095, 1096, 1098, 1100, 1101, 1104, 1109, 1111, 
 * 1112, 1113, 1114, 1116, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1126, 1127, 1129, 1131, 1132, 1133, 1134, 1135, 1137, 3082, 
 * 3104, 3109, 3135, 33769]
 */