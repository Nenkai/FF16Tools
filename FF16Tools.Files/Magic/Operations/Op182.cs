using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op182 : MagicOperationBase<Op182>, IOperationBase<Op182>
{
    public override MagicOperationType Type => MagicOperationType.Operation_182;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_134, 
        MagicPropertyType.Prop_136, 
        MagicPropertyType.Prop_3785, 
        MagicPropertyType.Prop_3802, 
        MagicPropertyType.Prop_6179,
        MagicPropertyType.Prop_6184
    ];
}
