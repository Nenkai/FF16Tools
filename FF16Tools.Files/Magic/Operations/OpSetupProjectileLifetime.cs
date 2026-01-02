using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpSetupProjectileLifetime : MagicOperationBase<OpSetupProjectileLifetime>, IOperationBase<OpSetupProjectileLifetime>
{
    public override MagicOperationType Type => MagicOperationType.Operation_35_SetupProjectileLifetime;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_ProjectileDuration, 
        MagicPropertyType.Prop_ProjectilaDurationRandomRange, 
        MagicPropertyType.Prop_OnNoImpactOperationGroupIdCallback, 
        MagicPropertyType.Prop_38,
        MagicPropertyType.Prop_1417
    ];
}
