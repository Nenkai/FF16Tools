using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op116 : MagicOperationBase<Op116>, IOperationBase<Op116>
{
    public override MagicOperationType Type => MagicOperationType.Operation_116;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.Prop_93, 
        MagicPropertyType.ProjectileMaxDistance, 
        MagicPropertyType.Prop_118, 
        MagicPropertyType.Prop_119,
        MagicPropertyType.Prop_120
    ];
}
