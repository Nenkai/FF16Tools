using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7187 : MagicOperationBase<Op7187>, IOperationBase<Op7187>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7187;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.ProjectileCreateGroundYOffset,
        MagicPropertyType.Prop_187
    ];
}
