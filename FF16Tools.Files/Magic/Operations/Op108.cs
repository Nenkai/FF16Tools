using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op108 : MagicOperationBase<Op108>, IOperationBase<Op108>
{
    public override MagicOperationType Type => MagicOperationType.Operation_108;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        // All default to 0
        MagicPropertyType.Prop_3, 
        MagicPropertyType.Prop_6, 
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate,
        MagicPropertyType.ProjectileSpeedMax,
        MagicPropertyType.HeightUnk,
    ];
}
