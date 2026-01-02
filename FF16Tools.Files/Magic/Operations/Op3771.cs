using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3771 : MagicOperationBase<Op3771>, IOperationBase<Op3771>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3771;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3, 
        MagicPropertyType.Prop_4, 
        MagicPropertyType.Prop_5, 
        MagicPropertyType.Prop8_SpeedStart, 
        MagicPropertyType.Prop_OnNoImpactOperationGroupIdCallback, 
        MagicPropertyType.Prop_74, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_117, 
        MagicPropertyType.Prop_3440, 
        MagicPropertyType.Prop_3441, 
        MagicPropertyType.Prop_3608, 
        MagicPropertyType.Prop_6339, 
        MagicPropertyType.Prop_6340, 
        MagicPropertyType.Prop_6471, 
        MagicPropertyType.Prop_6515,
        MagicPropertyType.Prop_6516
    ];
}
