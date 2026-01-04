using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op137 : MagicOperationBase<Op137>, IOperationBase<Op137>
{
    public override MagicOperationType Type => MagicOperationType.Operation_137;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.ProjectileHitboxRadiusStart,
        MagicPropertyType.Prop_117
    ];
}
