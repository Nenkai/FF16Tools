using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpInitialize : MagicOperationBase<OpInitialize>, IOperationBase<OpInitialize>
{
    public override MagicOperationType Type => MagicOperationType.Operation_51_Initialize;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileYOffset, 
        MagicPropertyType.ProjectileTrackingRotationRate, 
        MagicPropertyType.ProjectileCreationOpGroupId, 
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.Projectile1XYZOffset, 
        MagicPropertyType.Projectile2XYZOffset, 
        MagicPropertyType.Projectile3XYZOffset, 
        MagicPropertyType.Projectile4XYZOffset, 
        MagicPropertyType.Projectile5XYZOffset, 
        MagicPropertyType.Projectile6XYZOffset, 
        MagicPropertyType.Projectile7XYZOffset, 
        MagicPropertyType.Projectile8XYZOffset, 
        MagicPropertyType.Projectile9XYZOffset, 
        MagicPropertyType.Projectile10XYZOffset, 
        MagicPropertyType.Projectile11XYZOffset, 
        MagicPropertyType.Projectile12XYZOffset, 
        MagicPropertyType.MultiProjectileRandBoxExtend, 
        MagicPropertyType.MultiProjectileRandUniform, 
        MagicPropertyType.MultiProjectileMinRandRadius, 
        MagicPropertyType.NumProjectilesToSpawnRandom,
        MagicPropertyType.Prop_68_MagicId, 
        MagicPropertyType.Prop_69_TargetType, 
        MagicPropertyType.Prop_70_UnkJitterMaxAngleRadXYZ, 
        MagicPropertyType.Prop_71, 
        MagicPropertyType.ProjectileXYZOffset, 
        MagicPropertyType.Prop_73_Type, 
        MagicPropertyType.DelaySecBetweenProjectileCreation,
        MagicPropertyType.NumProjectilesToSpawn, // Defaults to 1
        MagicPropertyType.Prop_76, 
        MagicPropertyType.Prop_77, 
        MagicPropertyType.Prop_78, 
        MagicPropertyType.Prop_79_UnkAngleRadX, 
        MagicPropertyType.Prop_80_UnkAngleRadY, 
        MagicPropertyType.ActorEidForProjectileSource,
        MagicPropertyType.Prop_82, 
        MagicPropertyType.Prop_84, 
        MagicPropertyType.Prop_85,
        MagicPropertyType.Prop_114, 
        MagicPropertyType.Prop_185_UnkAngleRadZ, 
        MagicPropertyType.Prop_1102, 
        MagicPropertyType.Prop_1286, 
        MagicPropertyType.Prop_1433, 
        MagicPropertyType.Prop_1434, 
        MagicPropertyType.Prop_1458, 
        MagicPropertyType.Prop_1484_UnkJitterMaxAngleRadY, 
        MagicPropertyType.MultiProjectileMaxRandRadius, 
        MagicPropertyType.Prop_2396_LayoutInstanceId, 
        MagicPropertyType.Prop_2397_LayoutInstanceDistanceUnk, 
        MagicPropertyType.Prop_3916_Prop71Alt, 
        MagicPropertyType.Prop_3938_LayoutInstanceId,
        MagicPropertyType.Prop_3939,
        MagicPropertyType.Prop_4010_UnkJitterMaxAngleRadX, 
        MagicPropertyType.Prop_4011, 
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ, // Defaults to 100.0?
        MagicPropertyType.Prop_4102, // Defaults to 1.0?
        MagicPropertyType.Prop_4103, 
        MagicPropertyType.ProjectileRandXYZOffset, 
        MagicPropertyType.Prop_4142,
        MagicPropertyType.Prop_4143_UnkProjCount, // Defaults to 10
        MagicPropertyType.Prop_4144, 
        MagicPropertyType.Prop_5119, 
        MagicPropertyType.Prop_5945, 
        MagicPropertyType.Prop_5947,
        MagicPropertyType.Prop_6383, 
        MagicPropertyType.Prop_6596, 
        MagicPropertyType.Prop_6600_Req6596_UnkY, 
        MagicPropertyType.Prop_6913_Type, 
        MagicPropertyType.Prop_6975, 
        MagicPropertyType.Prop_7037, 
        MagicPropertyType.Prop_7186, 
        MagicPropertyType.Prop_7200_Prop66Alt,
        MagicPropertyType.Prop_7811
    ];
}
