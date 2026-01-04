using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5476 : MagicOperationBase<Op5476>, IOperationBase<Op5476>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5476;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.VFX_XYZOffset, 
        MagicPropertyType.Prop_93, 
        MagicPropertyType.Prop_5838,
        MagicPropertyType.Prop_5839
    ];
}
