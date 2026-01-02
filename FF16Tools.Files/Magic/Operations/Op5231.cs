using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5231 : MagicOperationBase<Op5231>, IOperationBase<Op5231>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5231;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_5232, 
        MagicPropertyType.Prop_5234, 
        MagicPropertyType.Prop_5235, 
        MagicPropertyType.Prop_5236, 
        MagicPropertyType.Prop_5237,
        MagicPropertyType.Prop_5323
    ];
}
