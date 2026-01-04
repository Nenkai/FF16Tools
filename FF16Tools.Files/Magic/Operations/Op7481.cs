using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7481 : MagicOperationBase<Op7481>, IOperationBase<Op7481>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7481;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_25, 
        MagicPropertyType.ProjectileOnHitAttackParamId,
        MagicPropertyType.ProjectileHitboxRadiusStart, 
        MagicPropertyType.Prop_81_EidId, 
        MagicPropertyType.Prop_997, 
        MagicPropertyType.Prop_3659, 
        MagicPropertyType.Prop_6087, 
        MagicPropertyType.Prop_6088, 
        MagicPropertyType.Prop_6257, 
        MagicPropertyType.Prop_7480, 
        MagicPropertyType.Prop_7482, 
        MagicPropertyType.Prop_7483, 
        MagicPropertyType.Prop_7484, 
        MagicPropertyType.Prop_7485,
        MagicPropertyType.Prop_7486
    ];
}
