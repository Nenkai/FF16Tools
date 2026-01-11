using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpSetupProjectileLifetime : OpSetupProjectileLifetimeBase<OpSetupProjectileLifetime>, IOperationBase<OpSetupProjectileLifetime>
{
    public override MagicOperationType Type => MagicOperationType.Operation_35_SetupProjectileLifetime;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } = BaseProperties;
}

public abstract class OpSetupProjectileLifetimeBase<T> : MagicOperationBase<T>
    where T : OpSetupProjectileLifetimeBase<T>, IOperationBase<T>
{
    protected static HashSet<MagicPropertyType> BaseProperties { get; set; } =
    [
        MagicPropertyType.ProjectileDuration,                    // Default 0.0
        MagicPropertyType.ProjectileDurationRandomRange,         // Default <0.0, 0.0, 0.0>
        MagicPropertyType.ProjectileNoImpactOpGroupIdCallback,   // Default 0
        MagicPropertyType.Prop_38,                               // Default false
        MagicPropertyType.ProjectileDurationWithDifficultyScale  // Default 0.0
    ];
}
