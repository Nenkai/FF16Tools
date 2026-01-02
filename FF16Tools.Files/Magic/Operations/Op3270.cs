using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3176 : MagicOperationBase<Op3176>, IOperationBase<Op3176>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3070;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3176, 
        MagicPropertyType.Prop_4071,
        MagicPropertyType.Prop_7102
    ];
}
