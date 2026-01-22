using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4784 : MagicOperationBase<Op4784>, IOperationBase<Op4784>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4784;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart,
        MagicPropertyType.ProjectileSpeedIncreaseRate,
        MagicPropertyType.Prop_24,
        MagicPropertyType.Prop_28,
        MagicPropertyType.Prop_30,
        MagicPropertyType.ProjectileOnHitAttackParamId,
        MagicPropertyType.ProjectileHitboxRadiusStart,
        MagicPropertyType.ProjectileHitboxRadiusIncreaseRate,
        MagicPropertyType.Projectile1XYZOffset,
        MagicPropertyType.Projectile2XYZOffset,
        MagicPropertyType.NumProjectilesToSpawn,
        MagicPropertyType.Prop_78,
        MagicPropertyType.ActorEidForProjectileSource,
        MagicPropertyType.ActorVFXAudioId,
        MagicPropertyType.OnFinishedOpGroupId,
        MagicPropertyType.Prop_104,
        MagicPropertyType.Prop_105,
        MagicPropertyType.Prop_109,
        MagicPropertyType.Prop_148,
        MagicPropertyType.Prop_188,
        MagicPropertyType.Prop_995,
        MagicPropertyType.Prop_2060,
        MagicPropertyType.Prop_3722_EidId,
        MagicPropertyType.Prop_3848,
        MagicPropertyType.Prop_3856,
        MagicPropertyType.Prop_4277,
        MagicPropertyType.Prop_4846,
    ];
}
