using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3515 : MagicOperationBase<Op3515>, IOperationBase<Op3515>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3515;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3, 
        MagicPropertyType.Prop8_SpeedStart, 
        MagicPropertyType.Prop9_SpeedIncreaseRate, 
        MagicPropertyType.Prop11_SpeedMax, 
        MagicPropertyType.Prop_12, 
        MagicPropertyType.Prop_28, 
        MagicPropertyType.Prop_122, 
        MagicPropertyType.Prop_123, 
        MagicPropertyType.Prop_2413, 
        MagicPropertyType.Prop_3903,
        MagicPropertyType.Prop_4008
    ];
}
