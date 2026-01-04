using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4998 : MagicOperationBase<Op4998>, IOperationBase<Op4998>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4998;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_26, 
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.Prop_74_Duration, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_4999,
        MagicPropertyType.Prop_5000,
        MagicPropertyType.Prop_6115
    ];
}
