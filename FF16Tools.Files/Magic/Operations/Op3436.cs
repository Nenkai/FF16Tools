using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3436 : MagicOperationBase<Op3436>, IOperationBase<Op3436>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3436;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_102
    ];
}
