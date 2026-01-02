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
        MagicPropertyType.Prop_32, 
        MagicPropertyType.Prop_ProjectileDuration, 
        MagicPropertyType.Prop_AttackParamId, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_43, 
        MagicPropertyType.Prop_44, 
        MagicPropertyType.Prop_45, 
        MagicPropertyType.Prop_46, 
        MagicPropertyType.Prop_47, 
        MagicPropertyType.Prop_48, 
        MagicPropertyType.Prop_49, 
        MagicPropertyType.Prop_1123, 
        MagicPropertyType.Prop_2227, 
        MagicPropertyType.Prop_2413, 
        MagicPropertyType.Prop_2414, 
        MagicPropertyType.Prop_2430,
        MagicPropertyType.Prop_4131, 
        MagicPropertyType.Prop_4132, 
        MagicPropertyType.Prop_5274, 
        MagicPropertyType.Prop_5275, 
        MagicPropertyType.Prop_5276, 
        MagicPropertyType.Prop_6800, 
        MagicPropertyType.Prop_6801,
        MagicPropertyType.Prop_6822
    ];
}
