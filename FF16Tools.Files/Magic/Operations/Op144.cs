using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op144 : MagicOperationBase<Op144>, IOperationBase<Op144>
{
    public override MagicOperationType Type => MagicOperationType.Operation_144;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileTrackingRotationRate, 
        MagicPropertyType.ProjectileDirectionAngles, 
        MagicPropertyType.ProjectileCreationOpGroupId, 
        MagicPropertyType.Distance, 
        MagicPropertyType.DelaySecBetweenProjectileCreation, 
        MagicPropertyType.NumProjectilesToSpawn, 
        MagicPropertyType.ActorEidForProjectileSource,
        MagicPropertyType.Prop_145
    ];
}
