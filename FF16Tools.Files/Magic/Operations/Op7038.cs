using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7038 : MagicOperationBase<Op7038>, IOperationBase<Op7038>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7038;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_1, 
        MagicPropertyType.Prop_7039, 
        MagicPropertyType.Prop_7040, 
        MagicPropertyType.Prop_7041,
        MagicPropertyType.Prop_7042
    ];
}
