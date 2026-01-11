using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op1587 : MagicOperationBase<Op1587>, IOperationBase<Op1587>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1587;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_15, 
        MagicPropertyType.Prop_24, 
        MagicPropertyType.ProjectileCreationOpGroupId, 
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.MultiProjectileRandBoxExtend, 
        MagicPropertyType.MultiProjectileRandUniform, 
        MagicPropertyType.MultiProjectileMinRandRadius, 
        MagicPropertyType.Prop_1286, 
        MagicPropertyType.Prop_1433, 
        MagicPropertyType.Prop_2906, 
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ, 
        MagicPropertyType.Prop_4102, 
        MagicPropertyType.Prop_4278, 
        MagicPropertyType.Prop_4279,
        MagicPropertyType.Prop_4383
    ];
}
