using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5059 : MagicOperationBase<Op5059>, IOperationBase<Op5059>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5059;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_ProjectileDuration
    ];
}
