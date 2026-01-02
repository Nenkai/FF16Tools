using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op2592 : MagicOperationBase<Op2592>, IOperationBase<Op2592>
{
    public override MagicOperationType Type => MagicOperationType.Operation_2592;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_ProjectileDuration, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_43, 
        MagicPropertyType.Prop_102,
        MagicPropertyType.Prop_148, 
        MagicPropertyType.Prop_149, 
        MagicPropertyType.Prop_150, 
        MagicPropertyType.Prop_1686,
        MagicPropertyType.Prop_1688,
        MagicPropertyType.Prop_1793
    ];
}
