using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpCreateTrap : OpSetupProjectileLifetimeBase<OpCreateTrap>, IOperationBase<OpCreateTrap>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6460_CreateTrap;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
        OpSetupProjectileLifetime.sSupportedProperties.Concat([
            MagicPropertyType.Prop_6459
        ])
        .ToHashSet();

}
