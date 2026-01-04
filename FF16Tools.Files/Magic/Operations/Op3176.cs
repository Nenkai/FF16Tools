using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3270 : MagicOperationBase<Op3270>, IOperationBase<Op3270>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3270;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.Prop_46,
        MagicPropertyType.Prop_102
    ];
}
