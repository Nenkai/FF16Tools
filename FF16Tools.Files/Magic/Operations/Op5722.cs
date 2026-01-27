using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5643 : MagicOperationBase<Op5643>, IOperationBase<Op5643>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5643;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.ProjectileSpeedMax, 
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.Distance, 
        MagicPropertyType.DistanceMax, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_3659,
        MagicPropertyType.Prop_5683, 
        MagicPropertyType.Prop_5684, 
        MagicPropertyType.Prop_6053, 
        MagicPropertyType.Prop_6087, 
        MagicPropertyType.Prop_6088, 
        MagicPropertyType.Prop_6089, 
        MagicPropertyType.Prop_6090,
        MagicPropertyType.Prop_6180
    ];
}
