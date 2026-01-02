using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3070 : MagicOperationBase<Op3070>, IOperationBase<Op3070>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3070;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_117, 
        MagicPropertyType.Prop_3077,
        MagicPropertyType.Prop_3113,
        MagicPropertyType.Prop_3114, 
        MagicPropertyType.Prop_3115, 
        MagicPropertyType.Prop_3116, 
        MagicPropertyType.Prop_3694, 
        MagicPropertyType.Prop_3695,
        MagicPropertyType.Prop_3696
    ];
}
