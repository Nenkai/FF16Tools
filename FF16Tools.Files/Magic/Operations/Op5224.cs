using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5224 : MagicOperationBase<Op5224>, IOperationBase<Op5224>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5224;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_42,
        MagicPropertyType.Prop_188
    ];
}
