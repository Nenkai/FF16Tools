using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4446 : Op1562Base<Op4446>, IOperationBase<Op4446>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4446;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
        Op1562.sSupportedProperties.Concat([
            MagicPropertyType.Prop_4447,
        ])
        .ToHashSet();
}
