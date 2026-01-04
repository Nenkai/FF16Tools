using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op169 : MagicOperationBase<Op169>, IOperationBase<Op169>
{
    public override MagicOperationType Type => MagicOperationType.Operation_169;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_102,
        MagicPropertyType.Prop_2764
    ];
}
