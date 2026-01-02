using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op100 : MagicOperationBase<Op100>, IOperationBase<Op100>
{
    public override MagicOperationType Type => MagicOperationType.Operation_100;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_AttackParamId,
    ];
}
