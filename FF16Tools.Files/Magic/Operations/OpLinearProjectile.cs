using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpLinearProjectile : MagicOperationBase<OpLinearProjectile>, IOperationBase<OpLinearProjectile>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1_LinearProjectile;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3,                                             // Default 0.0
        MagicPropertyType.Prop_4,                                             // Default 0
        MagicPropertyType.Prop_5,
        MagicPropertyType.Prop_6,                                             // Default 0.0
        MagicPropertyType.Prop_7,                                             // Default 0.0
        MagicPropertyType.ProjectileSpeedStart,                               // Default 0.0
        MagicPropertyType.ProjectileSpeedIncreaseRate,                        // Default 0.0
        MagicPropertyType.ProjectileSpeedStartRangeRand,                      // Default <0.0, 0.0, 0.0>
        MagicPropertyType.ProjectileSpeedMax,                                 // Default 0.0
        MagicPropertyType.ProjectileYOffset,                                  // Default 0.0
        MagicPropertyType.ProjectileNoTrackingTarget,                         // Default false
        MagicPropertyType.ProjectileTrackingRotationRate,                     // Default 0.0
        MagicPropertyType.Prop_15,                                            // Default false
        MagicPropertyType.ProjectileNormalizeXZTarget,                        // Default false
        MagicPropertyType.ProjectileYRandOffset,                              // Default 0.0
        MagicPropertyType.ProjectileBindPositionToSourceActor,                // Default false
        MagicPropertyType.ProjectileBindPositionToSourceActorNoProgress,      // Default false
        MagicPropertyType.ProjectileXSinWaveToTarget,                         // Default false
        MagicPropertyType.Prop_21_EidId,                                      // Default 0
        MagicPropertyType.Prop_22_VerticalAngleDegreesOffset,                 // Default 0.0
        MagicPropertyType.ProjectileDirectionAngles,                          // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Prop_24,                                            // Default 0
        MagicPropertyType.Prop_1269_LayoutInstanceId,                         // Default 0
        MagicPropertyType.ProjectileStopAtDist,                               // Default 0.0
        MagicPropertyType.Prop_2286,                                          // Default 0.0
        MagicPropertyType.Prop_2351,                                          // Default <-1.0, -1.0, -1.0>
        MagicPropertyType.Prop_2368,                                          // Default false
        MagicPropertyType.Prop_2856,                                          // Default false
        MagicPropertyType.Prop_3513,                                          // Default false
        MagicPropertyType.Prop_3514,                                          // Default 0.0
        MagicPropertyType.Prop_3544_EidId,                                    // Default 0
        MagicPropertyType.Prop_3605,                                          // Default false
        MagicPropertyType.Prop_3606,                                          // Default false
        MagicPropertyType.Prop_3722_EidId,                                    // Default 0
        MagicPropertyType.ProjectileOnExecuteActionId,                        // Default 0
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ,                    // Default 100.0
        MagicPropertyType.Prop_4102,                                          // Default 1.0
        MagicPropertyType.Prop_4128_UnkTargetType,                            // Default 0
        MagicPropertyType.ProjectileTranslation,                              // Default <0.0, 0.0, 1.0>
        MagicPropertyType.ProjectileSpeedRateRandRange,                       // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Prop_6776,                                          // Default false
        MagicPropertyType.Prop_6825,                                          // Default false
        MagicPropertyType.Prop_6826,                                          // Default false
        MagicPropertyType.Prop_6988                                           // Default false
    ];

}
