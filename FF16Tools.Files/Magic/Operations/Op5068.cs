using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5068 : MagicOperationBase<Op5068>, IOperationBase<Op5068>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5068;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3434, 
        MagicPropertyType.Prop_3440, 
        MagicPropertyType.Prop_3441,
        MagicPropertyType.Prop_3608, 
        MagicPropertyType.Prop_3609, 
        MagicPropertyType.Prop_3691, 
        MagicPropertyType.Prop_3962,
        MagicPropertyType.Prop_3963
    ];
}
