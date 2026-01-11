using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op111 : MagicOperationBase<Op111>, IOperationBase<Op111>
{
    public override MagicOperationType Type => MagicOperationType.Operation_111;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.VFXScale,
        MagicPropertyType.Prop_112, 
        MagicPropertyType.Prop_113, 
        MagicPropertyType.Prop_114,
    ];
}
