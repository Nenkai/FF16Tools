using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3847 : MagicOperationBase<Op3847>, IOperationBase<Op3847>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3847;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_28, 
        MagicPropertyType.Prop_30, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_3848, 
        MagicPropertyType.Prop_3856, 
        MagicPropertyType.Prop_4008, 
        MagicPropertyType.Prop_4845, 
        MagicPropertyType.Prop_4846, 
        MagicPropertyType.Prop_4847,
        MagicPropertyType.Prop_4848
    ];
}
