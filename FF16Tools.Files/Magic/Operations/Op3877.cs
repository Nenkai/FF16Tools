using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3877 : MagicOperationBase<Op3877>, IOperationBase<Op3877>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3877;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3897, 
        MagicPropertyType.Prop_3898, 
        MagicPropertyType.Prop_3899, 
        MagicPropertyType.Prop_3900,
        MagicPropertyType.Prop_3901
    ];
}
