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
        MagicPropertyType.ProjectileYOffset,                        // Default 0.0
        MagicPropertyType.ProjectileTrackingRotationRate,           // Default 0.0
        MagicPropertyType.ProjectileCreationOpGroupId,              // Default 0
        MagicPropertyType.VFX_XYZOffset,                            // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile1XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile2XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile3XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile4XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile5XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile6XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile7XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile8XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile9XYZOffset,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile10XYZOffset,                    // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile11XYZOffset,                    // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Projectile12XYZOffset,                    // Default <0.0, 0.0, 0.0>
        MagicPropertyType.MultiProjectileRandBoxExtend,             // Default <0.0, 0.0, 0.0>
        MagicPropertyType.MultiProjectileRandUniform,               // Default 0.0
        MagicPropertyType.MultiProjectileMinRandRadius,             // Default 0.0
        MagicPropertyType.NumProjectilesToSpawnRandom,              // Default 0
        MagicPropertyType.OnProjectilePlacedMagicId,                // Default 0
        MagicPropertyType.Prop_69_TargetType,                       // Default 0
        MagicPropertyType.Prop_70_UnkJitterMaxAngleRadXYZ,          // Default 0.0
        MagicPropertyType.Prop_71,                                  // Default <0.0, 0.0, 0.0>
        MagicPropertyType.ProjectileXYZOffset,                      // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Prop_73_Type,                             // Default 0
        MagicPropertyType.DelaySecBetweenProjectileCreation,        // Default 0.0
        MagicPropertyType.NumProjectilesToSpawn,                    // Default 1
        MagicPropertyType.Prop_76,                                  // Default false
        MagicPropertyType.Prop_77,                                  // Default 0.0
        MagicPropertyType.Prop_78,                                  // Default false
        MagicPropertyType.Prop_79_UnkAngleRadX,                     // Default 0.0
        MagicPropertyType.Prop_80_UnkAngleRadY,                     // Default 0.0
        MagicPropertyType.ActorEidForProjectileSource,              // Default 0
        MagicPropertyType.Prop_82,                                  // Default false
        MagicPropertyType.Prop_84,                                  // Default 0
        MagicPropertyType.Prop_85,
        MagicPropertyType.Prop_114,                                 // Default false
        MagicPropertyType.Prop_185_UnkAngleRadZ,                    // Default 0.0
        MagicPropertyType.Prop_1102,                                // Default false
        MagicPropertyType.Prop_1286,                                // Default 0.0
        MagicPropertyType.Prop_1433,
        MagicPropertyType.Prop_1434,                                // Default 0.0
        MagicPropertyType.Prop_1458,
        MagicPropertyType.Prop_1484_UnkJitterMaxAngleRadY,          // Default 0.0
        MagicPropertyType.MultiProjectileMaxRandRadius,             // Default 0.0
        MagicPropertyType.Prop_2396_LayoutInstanceId,               // Default 0
        MagicPropertyType.Prop_2397_LayoutInstanceDistanceUnk,      // Default 0.0
        MagicPropertyType.Prop_3916_Prop71Alt,                      // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Prop_3938_LayoutInstanceId,               // Default 0
        MagicPropertyType.Prop_3939,                                // Default false
        MagicPropertyType.Prop_4010_UnkJitterMaxAngleRadX,          // Default 0.0
        MagicPropertyType.Prop_4011,                                // Default 0.0
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ,          // Default 100.0
        MagicPropertyType.Prop_4102,                                // Default 1.0
        MagicPropertyType.Prop_4103,                                // Default false
        MagicPropertyType.ProjectileRandXYZOffset,                  // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Prop_4142,                                // Default false
        MagicPropertyType.Prop_4143_UnkProjCount,                   // Default 10
        MagicPropertyType.Prop_4144,                                // Default false
        MagicPropertyType.Prop_5119,                                // Default false
        MagicPropertyType.Prop_5945,                                // Default false
        MagicPropertyType.Prop_5947,                                // Default false
        MagicPropertyType.Prop_6383,
        MagicPropertyType.Prop_6596,                                // Default false
        MagicPropertyType.Prop_6600_Req6596_UnkY,                   // Default 0.0
        MagicPropertyType.Prop_6913_Type,                           // Default 0
        MagicPropertyType.Prop_6975,
        MagicPropertyType.Prop_7037,
        MagicPropertyType.Prop_7186,                                // Default false
        MagicPropertyType.Prop_7200_Prop66Alt,                      // Default false
        MagicPropertyType.Prop_7811                                 // Default false
    ];
}
