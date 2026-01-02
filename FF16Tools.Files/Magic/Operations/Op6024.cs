using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op6024 : MagicOperationBase<Op6024>, IOperationBase<Op6024>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6024;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_26, 
        MagicPropertyType.Prop_74, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_117, 
        MagicPropertyType.Prop_6050,
        MagicPropertyType.Prop_6208
    ];
}
