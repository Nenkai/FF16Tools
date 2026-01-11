using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5321 : MagicOperationBase<Op5321>, IOperationBase<Op5321>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5321;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileDirectionAngles, 
        MagicPropertyType.ProjectileCreationOpGroupId, 
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.Prop_73_Type, 
        MagicPropertyType.DelaySecBetweenProjectileCreation, 
        MagicPropertyType.NumProjectilesToSpawn, 
        MagicPropertyType.Prop_5322, 
        MagicPropertyType.Prop_5324, 
        MagicPropertyType.Prop_5325, 
        MagicPropertyType.Prop_5326, 
        MagicPropertyType.Prop_5327, 
        MagicPropertyType.Prop_5355,
        MagicPropertyType.Prop_5356,
        MagicPropertyType.Prop_5525
    ];
}
