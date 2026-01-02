using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3910 : MagicOperationBase<Op3910>, IOperationBase<Op3910>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3910;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_26, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_117, 
        MagicPropertyType.Prop_3911,
        MagicPropertyType.Prop_3912
    ];
}
