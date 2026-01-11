using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op1562 : Op1562Base<Op1562>, IOperationBase<Op1562>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1562;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } = BaseProperties;
}

public abstract class Op1562Base<T> : MagicOperationBase<T>
    where T : Op1562Base<T>, IOperationBase<T>
{
    protected static HashSet<MagicPropertyType> BaseProperties { get; set; } =
    [
        MagicPropertyType.VFX_XYZOffset,
        MagicPropertyType.ProjectileHitboxRadiusStart,
        MagicPropertyType.ProjectileHitboxRadiusIncreaseRate,
        MagicPropertyType.ProjectileHitboxMaxRadius,
        MagicPropertyType.Prop_45,
        MagicPropertyType.Prop_46,
        MagicPropertyType.ProjectileCreateGroundYOffset,
        MagicPropertyType.Prop_48,
        MagicPropertyType.MinTimeForHitboxActivation,
        MagicPropertyType.Prop_114,
        MagicPropertyType.Prop_2430
    ];
}
