using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5207 : MagicOperationBase<Op5207>, IOperationBase<Op5207>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5207;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileCreationOpGroupId, 
        MagicPropertyType.Distance,
        MagicPropertyType.NumProjectilesToSpawn
    ];
}
