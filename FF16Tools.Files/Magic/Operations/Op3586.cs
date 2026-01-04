using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3586 : MagicOperationBase<Op3586>, IOperationBase<Op3586>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3586;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.Prop_46, 
        MagicPropertyType.Prop_81_EidId, 
        MagicPropertyType.Prop_102, 
        MagicPropertyType.Prop_110_OpGroupId, 
        MagicPropertyType.Prop_148,
        MagicPropertyType.Prop_2599
    ];
}
