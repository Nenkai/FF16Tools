using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op1562 : MagicOperationBase<Op1562>, IOperationBase<Op1562>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1562;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_32, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_43, 
        MagicPropertyType.Prop_44, 
        MagicPropertyType.Prop_45, 
        MagicPropertyType.Prop_46, 
        MagicPropertyType.Prop_47, 
        MagicPropertyType.Prop_48, 
        MagicPropertyType.Prop_49, 
        MagicPropertyType.Prop_114,
        MagicPropertyType.Prop_2430
    ];
}
