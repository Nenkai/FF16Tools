using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op6460 : OpSetupProjectileLifetimeBase<Op6460>, IOperationBase<Op6460>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6460;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
        OpSetupProjectileLifetime.sSupportedProperties.Concat([
            MagicPropertyType.Prop_6459
        ])
        .ToHashSet();

}
