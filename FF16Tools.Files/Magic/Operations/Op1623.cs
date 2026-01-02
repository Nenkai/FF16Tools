using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op1623 : MagicOperationBase<Op1623>, IOperationBase<Op1623>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1623;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop8_SpeedStart, 
        MagicPropertyType.Prop_81, 
        MagicPropertyType.Prop_1818, 
        MagicPropertyType.Prop_6270, 
        MagicPropertyType.Prop_6271, 
        MagicPropertyType.Prop_6272, 
        MagicPropertyType.Prop_6273, 
        MagicPropertyType.Prop_6274,
        MagicPropertyType.Prop_6275
    ];
}
