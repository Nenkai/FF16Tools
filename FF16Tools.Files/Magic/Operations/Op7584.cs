using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7584 : MagicOperationBase<Op7584>, IOperationBase<Op7584>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7584;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_12, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_47, 
        MagicPropertyType.Prop_74, 
        MagicPropertyType.Prop_117, 
        MagicPropertyType.Prop_4101,
        MagicPropertyType.Prop_4102, 
        MagicPropertyType.Prop_5274, 
        MagicPropertyType.Prop_5275, 
        MagicPropertyType.Prop_7585, 
        MagicPropertyType.Prop_7586,
        MagicPropertyType.Prop_7587, 
        MagicPropertyType.Prop_7588, 
        MagicPropertyType.Prop_7812
    ];
}
