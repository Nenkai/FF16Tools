using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op183 : MagicOperationBase<Op183>, IOperationBase<Op183>
{
    public override MagicOperationType Type => MagicOperationType.Operation_183;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_26, 
        MagicPropertyType.Prop_81, 
        MagicPropertyType.Prop_84, 
        MagicPropertyType.Prop_85,
        MagicPropertyType.Prop_186
    ];
}
