using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4106 : MagicOperationBase<Op4106>, IOperationBase<Op4106>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4106;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.ProjectileSpeedMax, 
        MagicPropertyType.ProjectileDirectionAngles, 
        MagicPropertyType.Prop_24, 
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.DistanceStart, 
        MagicPropertyType.DistanceIncreaseRate, 
        MagicPropertyType.Prop_46, 
        MagicPropertyType.Prop_69_TargetType, 
        MagicPropertyType.ActorEidForProjectileSource, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_1686, 
        MagicPropertyType.Prop_1688, 
        MagicPropertyType.Prop_1689, 
        MagicPropertyType.Prop_1690, 
        MagicPropertyType.Prop_1999, 
        MagicPropertyType.Prop_2259, 
        MagicPropertyType.Prop_2260, 
        MagicPropertyType.Prop_3431, 
        MagicPropertyType.Prop_3432, 
        MagicPropertyType.Prop_3659, 
        MagicPropertyType.Prop_3681, 
        MagicPropertyType.Prop_3782, 
        MagicPropertyType.PullDistanceOnTargetted, 
        MagicPropertyType.Prop_4128_UnkTargetType,
        MagicPropertyType.Prop_4262, 
        MagicPropertyType.Prop_4263, 
        MagicPropertyType.Prop_4282, 
        MagicPropertyType.Prop_4291, 
        MagicPropertyType.Prop_4293, 
        MagicPropertyType.Prop_4294, 
        MagicPropertyType.Prop_4295, 
        MagicPropertyType.Prop_4297, 
        MagicPropertyType.Prop_4298, 
        MagicPropertyType.Prop_4442, 
        MagicPropertyType.Prop_4443, 
        MagicPropertyType.Prop_4540, 
        MagicPropertyType.Prop_4832,
        MagicPropertyType.Prop_4833
    ];
}
