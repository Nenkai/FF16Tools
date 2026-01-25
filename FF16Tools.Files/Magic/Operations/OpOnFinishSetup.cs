using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

/// <summary>
/// Called when the magic instance setup has finished and is about to be deinitialized (not related to magic actors terminating).
/// </summary>
public class OpOnFinishSetup : MagicOperationBase<OpOnFinishSetup>, IOperationBase<OpOnFinishSetup>
{
    public override MagicOperationType Type => MagicOperationType.Operation_39_OnFinishSetup;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.OnDestroyOpGroupIdCallback,   // Default 0 (none)
        MagicPropertyType.OnDestroyOpGroupIdCallback2,  // Default 0 (none)
        MagicPropertyType.Prop_4785                     // Default false
    ];
}
