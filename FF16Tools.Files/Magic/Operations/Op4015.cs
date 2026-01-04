using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4015 : MagicOperationBase<Op4015>, IOperationBase<Op4015>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4015;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileBindPositionToSourceActor, 
        MagicPropertyType.Prop_26, 
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.Prop_47, 
        MagicPropertyType.Prop_48, 
        MagicPropertyType.Prop_64, 
        MagicPropertyType.Prop_69_TargetType, 
        MagicPropertyType.Prop_74_Duration, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_93, 
        MagicPropertyType.Prop_114, 
        MagicPropertyType.Prop_1484, 
        MagicPropertyType.Prop_2430, 
        MagicPropertyType.Prop_3499, 
        MagicPropertyType.Prop_3544_EidId, 
        MagicPropertyType.Prop_3721, 
        MagicPropertyType.Prop_4010, 
        MagicPropertyType.Prop_4011, 
        MagicPropertyType.Prop_4614, 
        MagicPropertyType.Prop_5120,
        MagicPropertyType.Prop_5904
    ];
}
