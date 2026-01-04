using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3790 : MagicOperationBase<Op3790>, IOperationBase<Op3790>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3790;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileNoImpactOpGroupIdCallback, 
        MagicPropertyType.Prop_96_OpGroupId, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_1833_OpGroupId, 
        MagicPropertyType.Prop_2593,
        MagicPropertyType.Prop_3440
    ];
}
