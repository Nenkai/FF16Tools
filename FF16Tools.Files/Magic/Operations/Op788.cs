using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op788 : MagicOperationBase<Op788>, IOperationBase<Op788>
{
    public override MagicOperationType Type => MagicOperationType.Operation_788;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_29, 
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.Distance, 
        MagicPropertyType.Prop_69_TargetType, 
        MagicPropertyType.Prop_73_Type, 
        MagicPropertyType.ActorEidForProjectileSource, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_997, 
        MagicPropertyType.Prop_1025, 
        MagicPropertyType.Prop_1026, 
        MagicPropertyType.Prop_1490, 
        MagicPropertyType.Prop_1807, 
        MagicPropertyType.Prop_1959, 
        MagicPropertyType.Prop_1994, 
        MagicPropertyType.Prop_1997, 
        MagicPropertyType.Prop_1999, 
        MagicPropertyType.Prop_2000, 
        MagicPropertyType.Prop_2259, 
        MagicPropertyType.Prop_2260, 
        MagicPropertyType.Prop_2319, 
        MagicPropertyType.Prop_2528, 
        MagicPropertyType.Prop_2529, 
        MagicPropertyType.Prop_2604, 
        MagicPropertyType.Prop_2672, 
        MagicPropertyType.Prop_3078,
        MagicPropertyType.Prop_3079, 
        MagicPropertyType.Prop_3362, 
        MagicPropertyType.Prop_3363, 
        MagicPropertyType.Prop_3440, 
        MagicPropertyType.Prop_3658, 
        MagicPropertyType.Prop_3659, 
        MagicPropertyType.Prop_3681, 
        MagicPropertyType.Prop_3872, 
        MagicPropertyType.Prop_3906, 
        MagicPropertyType.Prop_3961, 
        MagicPropertyType.Prop_3962, 
        MagicPropertyType.Prop_3970, 
        MagicPropertyType.Prop_4131_AttackParamId, 
        MagicPropertyType.Prop_4132_AttackParamId, 
        MagicPropertyType.Prop_4427, 
        MagicPropertyType.Prop_4509, 
        MagicPropertyType.Prop_4829, 
        MagicPropertyType.Prop_5101, 
        MagicPropertyType.Prop_5125,
        MagicPropertyType.Prop_5274_AttackParamId, 
        MagicPropertyType.Prop_5275_AttackParamId, 
        MagicPropertyType.Prop_5312, 
        MagicPropertyType.Prop_6299, 
        MagicPropertyType.Prop_6376, 
        MagicPropertyType.Prop_7028, 
        MagicPropertyType.Prop_7121, 
        MagicPropertyType.Prop_7428, 
        MagicPropertyType.Prop_7429, 
        MagicPropertyType.Prop_7430,
        MagicPropertyType.Prop_7807
    ];
}
