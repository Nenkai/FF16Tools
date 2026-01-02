using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op2493 : MagicOperationBase<Op2493>, IOperationBase<Op2493>
{
    public override MagicOperationType Type => MagicOperationType.Operation_2493;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_81, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_1341,
        MagicPropertyType.Prop_2367
    ];
}
