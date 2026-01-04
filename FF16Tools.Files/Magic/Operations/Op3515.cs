using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3515 : MagicOperationBase<Op3515>, IOperationBase<Op3515>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3515;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3, 
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.ProjectileSpeedMax, 
        MagicPropertyType.ProjectileYOffsetMaybe, 
        MagicPropertyType.Prop_28, 
        MagicPropertyType.Prop_122, 
        MagicPropertyType.Prop_123, 
        MagicPropertyType.Prop_2413_UnkRateForProp47, 
        MagicPropertyType.Prop_3903,
        MagicPropertyType.Prop_4008
    ];
}
