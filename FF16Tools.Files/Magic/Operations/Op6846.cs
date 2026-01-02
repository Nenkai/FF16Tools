using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op6846 : MagicOperationBase<Op6846>, IOperationBase<Op6846>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6846;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_6851, 
        MagicPropertyType.Prop_6925, 
        MagicPropertyType.Prop_6926, 
        MagicPropertyType.Prop_6927,
        MagicPropertyType.Prop_6928
    ];
}
