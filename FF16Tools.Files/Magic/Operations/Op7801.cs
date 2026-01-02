using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7801 : MagicOperationBase<Op7801>, IOperationBase<Op7801>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7801;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_2061, 
        MagicPropertyType.Prop_2062, 
        MagicPropertyType.Prop_2063, 
        MagicPropertyType.Prop_2064, 
        MagicPropertyType.Prop_2065, 
        MagicPropertyType.Prop_2067,
        MagicPropertyType.Prop_2068,
        MagicPropertyType.Prop_2069, 
        MagicPropertyType.Prop_3791,
        MagicPropertyType.Prop_5989,
        MagicPropertyType.Prop_6454
    ];
}
