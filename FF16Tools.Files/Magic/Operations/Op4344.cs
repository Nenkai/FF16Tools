using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4344 : MagicOperationBase<Op4344>, IOperationBase<Op4344>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4344;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop8_SpeedStart, 
        MagicPropertyType.Prop_12, 
        MagicPropertyType.Prop_24, 
        MagicPropertyType.Prop_32, 
        MagicPropertyType.Prop_79, 
        MagicPropertyType.Prop_81, 
        MagicPropertyType.Prop_132,
        MagicPropertyType.Prop_1432,
        MagicPropertyType.Prop_1999, 
        MagicPropertyType.Prop_2318, 
        MagicPropertyType.Prop_3078,
        MagicPropertyType.Prop_3606,
        MagicPropertyType.Prop_4277
    ];
}
