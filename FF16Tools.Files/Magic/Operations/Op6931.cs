using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op6931 : MagicOperationBase<Op6931>, IOperationBase<Op6931>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6931;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileYOffset, 
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.NumProjectilesToSpawn, 
        MagicPropertyType.OnFinishedOpGroupId, 
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ, 
        MagicPropertyType.Prop_4102,
        MagicPropertyType.Prop_5838
    ];
}
