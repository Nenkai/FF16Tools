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
        MagicPropertyType.OnTargetHitOpGroupIdCallback, 
        MagicPropertyType.Prop_96_OpGroupId, 
        MagicPropertyType.Prop_97_OpGroupId, 
        MagicPropertyType.Prop_98_OpGroupId, 
        MagicPropertyType.Prop_99_OpGroupId, 
        MagicPropertyType.Prop_1432_OpGroupId, 
        MagicPropertyType.Prop_1833_OpGroupId,
        MagicPropertyType.Prop_1835_OpGroupId, 
        MagicPropertyType.Prop_1957, 
        MagicPropertyType.Prop_4327_OpGroupId, 
        MagicPropertyType.Prop_4343_OpGroupId, 
        MagicPropertyType.Prop_4387, 
        MagicPropertyType.Prop_4785, 
        MagicPropertyType.Prop_4786, 
        MagicPropertyType.Prop_5671_OpGroupId, 
        MagicPropertyType.Prop_6137_OpGroupId,
        MagicPropertyType.Prop_7099,
        MagicPropertyType.Prop_7231
    ];
}
