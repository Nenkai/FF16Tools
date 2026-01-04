using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3391 : MagicOperationBase<Op3391>, IOperationBase<Op3391>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3391;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_52, 
        MagicPropertyType.ActorVFX_XYZOffset, 
        MagicPropertyType.Prop_102,
        MagicPropertyType.Prop_1458
    ];
}
