using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op2366 : MagicOperationBase<Op2366>, IOperationBase<Op2366>
{
    public override MagicOperationType Type => MagicOperationType.Operation_2366;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_81_EidId, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_1341,
        MagicPropertyType.Prop_2367
    ];
}
