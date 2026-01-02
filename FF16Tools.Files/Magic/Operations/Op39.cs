using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op39 : MagicOperationBase<Op39>, IOperationBase<Op39>
{
    public override MagicOperationType Type => MagicOperationType.Operation_39;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_39, 
        MagicPropertyType.Prop_1503,
        MagicPropertyType.Prop_4785
    ];
}
