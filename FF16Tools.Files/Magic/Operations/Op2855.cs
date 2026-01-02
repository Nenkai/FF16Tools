using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op2855 : MagicOperationBase<Op2855>, IOperationBase<Op2855>
{
    public override MagicOperationType Type => MagicOperationType.Operation_2855;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_15, 
        MagicPropertyType.Prop_16, 
        MagicPropertyType.Prop_23, 
        MagicPropertyType.Prop_79, 
        MagicPropertyType.Prop_80, 
        MagicPropertyType.Prop_185, 
        MagicPropertyType.Prop_2856, 
        MagicPropertyType.Prop_2857, 
        MagicPropertyType.Prop_2858, 
        MagicPropertyType.Prop_2859, 
        MagicPropertyType.Prop_2860, 
        MagicPropertyType.Prop_2977, 
        MagicPropertyType.Prop_2978, 
        MagicPropertyType.Prop_2979, 
        MagicPropertyType.Prop_3170, 
        MagicPropertyType.Prop_3495, 
        MagicPropertyType.Prop_3497, 
        MagicPropertyType.Prop_3498, 
        MagicPropertyType.Prop_3499,
        MagicPropertyType.Prop_4002, 
        MagicPropertyType.Prop_4014, 
        MagicPropertyType.Prop_5841,
        MagicPropertyType.Prop_7463
    ];
}
