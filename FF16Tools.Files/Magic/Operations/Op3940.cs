using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3940 : Op3433Base<Op3940>, IOperationBase<Op3940>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3940;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
        Op3433.sSupportedProperties.Concat([
            MagicPropertyType.NumProjectilesToSpawn,
            MagicPropertyType.Prop_5034,
        ])
        .ToHashSet();
}


