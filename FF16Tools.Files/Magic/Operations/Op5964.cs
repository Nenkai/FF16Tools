using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5964 : MagicOperationBase<Op5964>, IOperationBase<Op5964>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5964;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.ProjectileSpeedMax,
        MagicPropertyType.ProjectileDirectionAngles,
        MagicPropertyType.Prop_25, 
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.ProjectileOnHitAttackParamId, 
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.Prop_93, 
        MagicPropertyType.Prop_109,
        MagicPropertyType.Prop_3659,
        MagicPropertyType.Prop_3782, 
        MagicPropertyType.Prop_4999, 
        MagicPropertyType.Prop_5000, 
        MagicPropertyType.Prop_5970, 
        MagicPropertyType.Prop_5971, 
        MagicPropertyType.Prop_5972, 
        MagicPropertyType.Prop_5973, 
        MagicPropertyType.Prop_5974, 
        MagicPropertyType.Prop_6087, 
        MagicPropertyType.Prop_6088,
        MagicPropertyType.Prop_6203, 
        MagicPropertyType.Prop_6204, 
        MagicPropertyType.Prop_6257, 
        MagicPropertyType.Prop_6276,
        MagicPropertyType.Prop_6277
    ];
}
