using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3294 : MagicOperationBase<Op3294>, IOperationBase<Op3294>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3294;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_26, 
        MagicPropertyType.Prop_VFXScale, 
        MagicPropertyType.Prop_32,
        MagicPropertyType.Prop_132
    ];
}
