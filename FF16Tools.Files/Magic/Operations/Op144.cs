using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op144 : MagicOperationBase<Op144>, IOperationBase<Op144>
{
    public override MagicOperationType Type => MagicOperationType.Operation_144;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_14_UnkMaxAngleRad, 
        MagicPropertyType.ProjectileDirectionAngles, 
        MagicPropertyType.Prop_26, 
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.Prop_74_Duration, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_81_EidId,
        MagicPropertyType.Prop_145
    ];
}
