using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op1841 : Op1562Base<Op1841>, IOperationBase<Op1841>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1841;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
        Op1562.sSupportedProperties.Concat([
            MagicPropertyType.Prop_2575
        ])
        .ToHashSet();
}
