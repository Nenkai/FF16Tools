using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4784 : MagicOperationBase<Op4784>, IOperationBase<Op4784>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4784;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_42
    ];
}
