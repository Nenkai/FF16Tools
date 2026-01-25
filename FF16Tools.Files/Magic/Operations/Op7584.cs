using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7584 : MagicOperationBase<Op7584>, IOperationBase<Op7584>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7584;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileYOffset, 
        MagicPropertyType.DistanceStart, 
        MagicPropertyType.HeightUnk, 
        MagicPropertyType.DelaySecBetweenProjectileCreation, 
        MagicPropertyType.ProjectileMaxDistance, 
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ,
        MagicPropertyType.Prop_4102, 
        MagicPropertyType.Prop_5274_AttackParamId, 
        MagicPropertyType.Prop_5275_AttackParamId, 
        MagicPropertyType.Prop_7585, 
        MagicPropertyType.Prop_7586,
        MagicPropertyType.Prop_7587, 
        MagicPropertyType.Prop_7588, 
        MagicPropertyType.Prop_7812
    ];
}
