using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpSetupProjectileHitboxAttack : MagicOperationBase<OpSetupProjectileHitboxAttack>, IOperationBase<OpSetupProjectileHitboxAttack>
{
    public override MagicOperationType Type => MagicOperationType.Operation_40_SetupProjectileHitboxAttack;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.VFX_XYZOffset,                            // Default <0.0, 0.0, 0.0>
        MagicPropertyType.ProjectileDuration,                       // Default 0.0
        MagicPropertyType.ProjectileOnHitAttackParamId,             // Default 0
        MagicPropertyType.ProjectileHitboxRadiusStart,              // Default 0
        MagicPropertyType.ProjectileHitboxRadiusIncreaseRate,       // Default 0
        MagicPropertyType.ProjectileHitboxMaxRadius,                // Default 0.0
        MagicPropertyType.Prop_45,                                  // Default 0
        MagicPropertyType.Prop_46,                                  // Default 0.0
        MagicPropertyType.ProjectileCreateGroundYOffset,                                  // Default 0.0
        MagicPropertyType.Prop_48,                                  // Default 0.0
        MagicPropertyType.MinTimeForHitboxActivation,               // Default 0.0
        MagicPropertyType.Prop_1123,                                // Default false
        MagicPropertyType.Prop_2227,                                // Default 0
        MagicPropertyType.Prop_2413_UnkRateForProp47,               // Default 0.0
        MagicPropertyType.Prop_2414_UnkMaxForProp47,                // Default 0.0
        MagicPropertyType.Prop_2430,                                // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Prop_4131_AttackParamId,                  // Default 0
        MagicPropertyType.Prop_4132_AttackParamId,                  // Default 0
        MagicPropertyType.Prop_5274_AttackParamId,                  // Default 0
        MagicPropertyType.Prop_5275_AttackParamId,                  // Default 0
        MagicPropertyType.MinSkillUpgradeLevelForAttack_5274_5275,  // Default 2
        MagicPropertyType.Prop_6800_UnkIncreaseRateForProp48,       // Default 0.0
        MagicPropertyType.Prop_6800_UnkMaxForProp48,                // Default 0.0
        MagicPropertyType.Prop_6822_AttackParamId                   // Default 0
    ];
}
