using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3660 : MagicOperationBase<Op3660>, IOperationBase<Op3660>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3660;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_32, 
        MagicPropertyType.Prop_74, 
        MagicPropertyType.Prop_102,
        MagicPropertyType.Prop_2318
    ];
}
