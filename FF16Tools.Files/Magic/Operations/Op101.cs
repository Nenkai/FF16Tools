using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op101 : MagicOperationBase<Op101>, IOperationBase<Op101>
{
    public override MagicOperationType Type => MagicOperationType.Operation_101;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_102
    ];
}
