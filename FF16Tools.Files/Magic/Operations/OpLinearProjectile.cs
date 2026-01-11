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
        MagicPropertyType.Prop_3, 
        MagicPropertyType.Prop_4, 
        MagicPropertyType.Prop_5, 
        MagicPropertyType.Prop_6, 
        MagicPropertyType.Prop_7, 
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.ProjectileSpeedStartRangeRand,
        MagicPropertyType.ProjectileSpeedMax, 
        MagicPropertyType.ProjectileYOffset, 
        MagicPropertyType.ProjectileNoTrackingTarget, 
        MagicPropertyType.ProjectileTrackingRotationRate, 
        MagicPropertyType.Prop_15, 
        MagicPropertyType.Prop_16, 
        MagicPropertyType.ProjectileYRandOffset, 
        MagicPropertyType.ProjectileBindPositionToSourceActor, 
        MagicPropertyType.ProjectileBindPositionToSourceActorNoProgress, 
        MagicPropertyType.ProjectileXSinWaveToTarget, 
        MagicPropertyType.Prop_21_EidId, 
        MagicPropertyType.Prop_22_VerticalAngleDegreesOffset,
        MagicPropertyType.ProjectileDirectionAngles, 
        MagicPropertyType.Prop_24,
        MagicPropertyType.Prop_1269_LayoutInstanceId, 
        MagicPropertyType.ProjectileStopAtDist, 
        MagicPropertyType.Prop_2286, 
        MagicPropertyType.Prop_2351, 
        MagicPropertyType.Prop_2368, 
        MagicPropertyType.Prop_2856, 
        MagicPropertyType.Prop_3513, 
        MagicPropertyType.Prop_3514, 
        MagicPropertyType.Prop_3544_EidId,
        MagicPropertyType.Prop_3605, 
        MagicPropertyType.Prop_3606, 
        MagicPropertyType.Prop_3722_EidId, 
        MagicPropertyType.ProjectileOnExecuteActionId, 
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ, 
        MagicPropertyType.Prop_4102, 
        MagicPropertyType.Prop_4128_UnkTargetType, 
        MagicPropertyType.ProjectileTranslation, 
        MagicPropertyType.ProjectileSpeedRateRandRange,
        MagicPropertyType.Prop_6776, 
        MagicPropertyType.Prop_6825, 
        MagicPropertyType.Prop_6826,
        MagicPropertyType.Prop_6988
    ];

}
