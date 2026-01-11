using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3391 : MagicOperationBase<Op3391>, IOperationBase<Op3391>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3391;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Projectile1XYZOffset,   // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile2XYZOffset,   // Default <0.0, 0.0, 0.0>
        MagicPropertyType.OnFinishedOpGroupId,               // Default 0
        MagicPropertyType.Prop_1458,              // Default 0
        MagicPropertyType.Prop_5685,              // Code doesn't parse this? But Magic 821 has it?
    ];
}
