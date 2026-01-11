using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op92 : MagicOperationBase<Op92>, IOperationBase<Op92>
{
    public override MagicOperationType Type => MagicOperationType.Operation_92;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileCreationOpGroupId, 
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.Prop_39, 
        MagicPropertyType.Prop_70_UnkJitterMaxAngleRadXYZ, 
        MagicPropertyType.DelaySecBetweenProjectileCreation, 
        MagicPropertyType.NumProjectilesToSpawn, 
        MagicPropertyType.Prop_76, 
        MagicPropertyType.Prop_93, 
        MagicPropertyType.Prop_1791,
        MagicPropertyType.Prop_3721, 
        MagicPropertyType.Prop_3746, 
        MagicPropertyType.Prop_3747, 
        MagicPropertyType.Prop_6745, 
        MagicPropertyType.Prop_7535, 
        MagicPropertyType.Prop_7536, 
        MagicPropertyType.Prop_7537, 
        MagicPropertyType.Prop_7538, 
        MagicPropertyType.Prop_7539, 
        MagicPropertyType.Prop_7540, 
        MagicPropertyType.Prop_7541, 
        MagicPropertyType.Prop_7542,
        MagicPropertyType.Prop_7543
    ];
}
