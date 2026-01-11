using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4828 : MagicOperationBase<Op4828>, IOperationBase<Op4828>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4828;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileXSinWaveToTarget, 
        MagicPropertyType.ProjectileDirectionAngles, 
        MagicPropertyType.Prop_24, 
        MagicPropertyType.Prop_30, 
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.Projectile1XYZOffset, 
        MagicPropertyType.Prop_71, 
        MagicPropertyType.ActorVFXAudioId, 
        MagicPropertyType.Prop_105,
        MagicPropertyType.Prop_106,
        MagicPropertyType.Prop_119, 
        MagicPropertyType.Prop_132, 
        MagicPropertyType.Prop_172, 
        MagicPropertyType.Prop_187, 
        MagicPropertyType.Prop_1123,
        MagicPropertyType.Prop_1490, 
        MagicPropertyType.Prop_1688, 
        MagicPropertyType.Prop_1689, 
        MagicPropertyType.Prop_1690, 
        MagicPropertyType.Prop_2060, 
        MagicPropertyType.Prop_2856, 
        MagicPropertyType.Prop_2857, 
        MagicPropertyType.Prop_3431, 
        MagicPropertyType.Prop_3432,
        MagicPropertyType.Prop_3659, 
        MagicPropertyType.Prop_5747, 
        MagicPropertyType.Prop_5932,
        MagicPropertyType.Prop_5958
    ];
}
