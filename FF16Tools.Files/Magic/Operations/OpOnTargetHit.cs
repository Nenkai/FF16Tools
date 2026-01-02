using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpOnTargetHit : MagicOperationBase<OpOnTargetHit>, IOperationBase<OpOnTargetHit>
{
    public override MagicOperationType Type => MagicOperationType.Operation_94_OnTargetHit;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop95_OnTargetHitOperationGroupIdCallback, 
        MagicPropertyType.Prop_96, 
        MagicPropertyType.Prop_97, 
        MagicPropertyType.Prop_98, 
        MagicPropertyType.Prop_99, 
        MagicPropertyType.Prop_1432, 
        MagicPropertyType.Prop_1833,
        MagicPropertyType.Prop_1835, 
        MagicPropertyType.Prop_1957, 
        MagicPropertyType.Prop_4327, 
        MagicPropertyType.Prop_4343, 
        MagicPropertyType.Prop_4387, 
        MagicPropertyType.Prop_4785, 
        MagicPropertyType.Prop_4786, 
        MagicPropertyType.Prop_5671, 
        MagicPropertyType.Prop_6137,
        MagicPropertyType.Prop_7099,
        MagicPropertyType.Prop_7231
    ];
}
