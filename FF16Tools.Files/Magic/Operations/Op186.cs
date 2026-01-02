using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op186 : MagicOperationBase<Op186>, IOperationBase<Op186>
{
    public override MagicOperationType Type => MagicOperationType.Operation_186;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_117, 
        MagicPropertyType.Prop_187,
        MagicPropertyType.Prop_188
    ];
}
