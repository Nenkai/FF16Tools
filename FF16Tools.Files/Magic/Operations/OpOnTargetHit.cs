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
        MagicPropertyType.OnTargetHitOpGroupIdCallback,    // Default 0
        MagicPropertyType.Prop_96_OpGroupId,               // Default 0
        MagicPropertyType.Prop_97_OpGroupId,               // Default 0
        MagicPropertyType.Prop_98_OpGroupId,               // Default 0
        MagicPropertyType.Prop_99_OpGroupId,               // Default 0
        MagicPropertyType.Prop_1432_OpGroupId,             // Default 0
        MagicPropertyType.Prop_1833_OpGroupId,             // Default 0
        MagicPropertyType.Prop_1835_OpGroupId,             // Default 0
        MagicPropertyType.Prop_1957,                       // Default true
        MagicPropertyType.Prop_4327_OpGroupId,             // Default 0
        MagicPropertyType.Prop_4343_OpGroupId,             // Default 0
        MagicPropertyType.Prop_4387,                       // Default false
        MagicPropertyType.Prop_4785,                       // Default false
        MagicPropertyType.Prop_4786,                       // Default false
        MagicPropertyType.Prop_5671_OpGroupId,             // Default 0
        MagicPropertyType.Prop_6137_OpGroupId,             // Default 0
        MagicPropertyType.Prop_7099,                       // Default false
        MagicPropertyType.Prop_7231                        // Default false
    ];
}
