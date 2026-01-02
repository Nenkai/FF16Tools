using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op133 : MagicOperationBase<Op133>, IOperationBase<Op133>
{
    public override MagicOperationType Type => MagicOperationType.Operation_133;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_134
    ];
}
