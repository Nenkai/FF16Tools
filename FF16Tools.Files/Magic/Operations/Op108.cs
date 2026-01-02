using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op108 : MagicOperationBase<Op108>, IOperationBase<Op108>
{
    public override MagicOperationType Type => MagicOperationType.Operation_108;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_AttackParamId, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_106,
        MagicPropertyType.Prop_107
    ];
}
