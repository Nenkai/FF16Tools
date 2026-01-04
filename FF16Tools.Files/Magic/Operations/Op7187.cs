using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7292 : MagicOperationBase<Op7292>, IOperationBase<Op7292>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7292;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.ProjectileHitboxRadiusIncreaseRate, 
        MagicPropertyType.ProjectileHitboxMaxRadius, 
        MagicPropertyType.MinTimeForHitboxActivation, 
        MagicPropertyType.Prop_7293, 
        MagicPropertyType.Prop_7294, 
        MagicPropertyType.Prop_7300,
        MagicPropertyType.Prop_7610
    ];
}
