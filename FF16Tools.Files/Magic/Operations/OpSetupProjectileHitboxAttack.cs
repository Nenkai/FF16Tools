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
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.ProjectileDuration, 
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.ProjectileHitboxRadiusIncreaseRate, 
        MagicPropertyType.ProjectileHitboxMaxRadius, 
        MagicPropertyType.Prop_45, 
        MagicPropertyType.Prop_46, 
        MagicPropertyType.Prop_47, 
        MagicPropertyType.Prop_48, 
        MagicPropertyType.MinTimeForHitboxActivation, 
        MagicPropertyType.Prop_1123, 
        MagicPropertyType.Prop_2227, 
        MagicPropertyType.Prop_2413_UnkRateForProp47, 
        MagicPropertyType.Prop_2414_UnkMaxForProp47, 
        MagicPropertyType.Prop_2430,
        MagicPropertyType.Prop_4131_AttackParamId, 
        MagicPropertyType.Prop_4132_AttackParamId, 
        MagicPropertyType.Prop_5274_AttackParamId, 
        MagicPropertyType.Prop_5275_AttackParamId, 
        MagicPropertyType.MinSkillUpgradeLevelForAttack_5274_5275, 
        MagicPropertyType.Prop_6800_UnkIncreaseRateForProp48, 
        MagicPropertyType.Prop_6800_UnkMaxForProp48,
        MagicPropertyType.Prop_6822_AttackParamId
    ];
}
