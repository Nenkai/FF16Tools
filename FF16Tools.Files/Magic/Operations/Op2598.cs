using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op2598 : MagicOperationBase<Op2598>, IOperationBase<Op2598>
{
    public override MagicOperationType Type => MagicOperationType.Operation_2598;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_102, 
        MagicPropertyType.Prop_2599,
        MagicPropertyType.Prop_2630
    ];
}
