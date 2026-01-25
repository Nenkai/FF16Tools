using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5035 : MagicOperationBase<Op5035>, IOperationBase<Op5035>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5035;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileDuration, 
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.DistanceStart, 
        MagicPropertyType.Prop_46, 
        MagicPropertyType.DelaySecond, 
        MagicPropertyType.Projectile1XYZOffset, 
        MagicPropertyType.Projectile2XYZOffset, 
        MagicPropertyType.Projectile3XYZOffset, 
        MagicPropertyType.Prop_3078, 
        MagicPropertyType.Prop_3362, 
        MagicPropertyType.Prop_5036, 
        MagicPropertyType.Prop_5037, 
        MagicPropertyType.Prop_5038, 
        MagicPropertyType.Prop_5039, 
        MagicPropertyType.Prop_5040,
        MagicPropertyType.Prop_5041
    ];
}
