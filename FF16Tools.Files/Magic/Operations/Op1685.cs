using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op1685 : MagicOperationBase<Op1685>, IOperationBase<Op1685>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1685;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.ProjectileSpeedMax, 
        MagicPropertyType.ProjectileBindPositionToSourceActor, 
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.DistanceStart, 
        MagicPropertyType.DistanceIncreaseRate, 
        MagicPropertyType.Prop_114, 
        MagicPropertyType.Prop_1686, 
        MagicPropertyType.Prop_1687, 
        MagicPropertyType.Prop_1688, 
        MagicPropertyType.Prop_1689, 
        MagicPropertyType.Prop_1690, 
        MagicPropertyType.Prop_1697, 
        MagicPropertyType.Prop_1698,
        MagicPropertyType.Prop_1704
    ];
}
