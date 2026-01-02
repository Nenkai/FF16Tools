using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4446 : Op1841
{
    public override MagicOperationType Type => MagicOperationType.Operation_4446;
    public static new HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
        Op1841.sSupportedProperties.Concat([
            MagicPropertyType.Prop_4447,
        ])
        .ToHashSet();
}
