using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op6118 : MagicOperationBase<Op6118>, IOperationBase<Op6118>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6118;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_26, 
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.Prop_74_Duration, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_6119, 
        MagicPropertyType.Prop_6120, 
        MagicPropertyType.Prop_6121,
        MagicPropertyType.Prop_6122
    ];
}
