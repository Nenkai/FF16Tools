using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op121 : MagicOperationBase<Op121>, IOperationBase<Op121>
{
    public override MagicOperationType Type => MagicOperationType.Operation_121;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.Prop_102, 
        MagicPropertyType.Prop_122, 
        MagicPropertyType.Prop_123, 
        MagicPropertyType.Prop_124, 
        MagicPropertyType.Prop_125, 
        MagicPropertyType.Prop_126,
        MagicPropertyType.Prop_127
    ];
}
