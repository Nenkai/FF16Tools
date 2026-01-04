using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op103 : MagicOperationBase<Op103>, IOperationBase<Op103>
{
    public override MagicOperationType Type => MagicOperationType.Operation_103;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_106,
        MagicPropertyType.Prop_107
    ];
}
