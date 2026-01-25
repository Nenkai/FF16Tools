using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpCreateReactiveAoETrap : MagicOperationBase<OpCreateReactiveAoETrap>, IOperationBase<OpCreateReactiveAoETrap>
{
    public override MagicOperationType Type => MagicOperationType.Operation_CreateReactiveAoETrap;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart,                 // Default 1.0
        MagicPropertyType.VFX_XYZOffset,                        // Default <0.0, 0.0, 0.0>
        MagicPropertyType.ProjectileDuration,                   // Default 0.0
        MagicPropertyType.DistanceStart,                        // Default 1.0
        MagicPropertyType.HeightUnk,                            // Default 1.25
        MagicPropertyType.OnProjectilePlacedMagicId,            // Default 0
        MagicPropertyType.DelaySecBetweenProjectileCreation,    // Default 0.1 
        MagicPropertyType.NumProjectilesToSpawn,                // Default 1
        MagicPropertyType.ActorEidForProjectileSource,          // Default 0 
        MagicPropertyType.OnFinishedOpGroupId,                  // Default 0
        MagicPropertyType.ProjectileMaxDistance,                // Default 1.0
        MagicPropertyType.Prop_2060,                            // Default 2.0
        MagicPropertyType.PullDistanceOnTargetted,              // Default 0.75
        MagicPropertyType.Prop_4436,                            // Default 0
        MagicPropertyType.Prop_6287,                            // Default 6287
        MagicPropertyType.Prop_6459,                            // Default false
        MagicPropertyType.Prop_6831                             // Default 50.0
    ];
}
