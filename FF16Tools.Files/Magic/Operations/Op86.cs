using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op86 : MagicOperationBase<Op86>, IOperationBase<Op86>
{
    public override MagicOperationType Type => MagicOperationType.Operation_86;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_26, 
        MagicPropertyType.Prop_74, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_76,
        MagicPropertyType.Prop_84
    ];
}
