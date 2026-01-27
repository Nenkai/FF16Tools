using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op6942 : MagicOperationBase<Op6942>, IOperationBase<Op6942>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6942;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.Distance, 
        MagicPropertyType.ActorEidForProjectileSource, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_997, 
        MagicPropertyType.Prop_3078, 
        MagicPropertyType.Prop_3079, 
        MagicPropertyType.Prop_3362,
        MagicPropertyType.Prop_6299
    ];
}
