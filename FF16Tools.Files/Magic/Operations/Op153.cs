using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op153 : MagicOperationBase<Op153>, IOperationBase<Op153>
{
    public override MagicOperationType Type => MagicOperationType.Operation_153;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.MultiProjectileRandUniform, 
        MagicPropertyType.DelaySecBetweenProjectileCreation, 
        MagicPropertyType.NumProjectilesToSpawn, 
        MagicPropertyType.Prop_102, 
        MagicPropertyType.Prop_2919, 
        MagicPropertyType.Prop_2920,
        MagicPropertyType.Prop_2921
    ];
}
