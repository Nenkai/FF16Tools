using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3781 : MagicOperationBase<Op3781>, IOperationBase<Op3781>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3781;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.Distance, 
        MagicPropertyType.OnProjectilePlacedMagicId, 
        MagicPropertyType.DelaySecBetweenProjectileCreation, 
        MagicPropertyType.NumProjectilesToSpawn, 
        MagicPropertyType.Prop_132,
        MagicPropertyType.Prop_3782
    ];
}
