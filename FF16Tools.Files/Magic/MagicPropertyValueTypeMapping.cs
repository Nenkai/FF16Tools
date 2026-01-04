using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FF16Tools.Files.Magic.Factories;

namespace FF16Tools.Files.Magic;

public class MagicPropertyValueTypeMapping
{
    public static Dictionary<MagicPropertyType, MagicPropertyValueType> TypeToValueType { get; private set; } = new()
    {
        [MagicPropertyType.Prop_2] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_3] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop8_SpeedStart] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop13_NoTrackingTarget] = MagicPropertyValueType.BoolValue,
        [MagicPropertyType.Prop14_UnkMaxAngleRad] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop22_VerticalAngleDegreesOffset] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_26] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_VFXAudioId] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_30] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_VFXScale] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_ProjectileDuration] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_ProjectileDurationRandomRange] = MagicPropertyValueType.Vec3Value,
        [MagicPropertyType.Prop_OnNoImpactOperationGroupIdCallback] = MagicPropertyValueType.OperationGroupIdValue,
        [MagicPropertyType.Prop_42] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_45] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_46] = MagicPropertyValueType.FloatValue,
        [MagicPropertyType.Prop_AttackParamId] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_73] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_81] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_89] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop95_OnTargetHitOperationGroupIdCallback] = MagicPropertyValueType.OperationGroupIdValue,
        [MagicPropertyType.Prop_2227] = MagicPropertyValueType.IntValue,
        [MagicPropertyType.Prop_2575] = MagicPropertyValueType.IntValue,
    };


    /// <summary>
    /// Used for mapping additional properties not supported by default.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="valueType"></param>
    public static void Add(MagicPropertyType type, MagicPropertyValueType valueType)
    {
        TypeToValueType.TryAdd(type, valueType);
    }
}
