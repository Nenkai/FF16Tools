using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op1470 : MagicOperationBase<Op1470>, IOperationBase<Op1470>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1470;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.ProjectileSpeedMax, 
        MagicPropertyType.Prop_47, 
        MagicPropertyType.Prop_93, 
        MagicPropertyType.Prop_188,
        MagicPropertyType.Prop_1063
    ];
}
