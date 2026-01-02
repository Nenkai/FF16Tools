using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op139 : MagicOperationBase<Op139>, IOperationBase<Op139>
{
    public override MagicOperationType Type => MagicOperationType.Operation_139;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_23, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_43, 
        MagicPropertyType.Prop_44, 
        MagicPropertyType.Prop_114, 
        MagicPropertyType.Prop_140, 
        MagicPropertyType.Prop_3349,
        MagicPropertyType.Prop_4128
    ];
}
