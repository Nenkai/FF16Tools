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
        MagicPropertyType.ProjectileNormalizeXZTarget, 
        MagicPropertyType.ProjectileDirectionAngles, 
        MagicPropertyType.Prop_79_UnkAngleRadX, 
        MagicPropertyType.Prop_80_UnkAngleRadY, 
        MagicPropertyType.Prop_185_UnkAngleRadZ, 
        MagicPropertyType.Prop_2856, 
        MagicPropertyType.Prop_2857, 
        MagicPropertyType.Prop_2858, 
        MagicPropertyType.Prop_2859, 
        MagicPropertyType.Prop_2860, 
        MagicPropertyType.Prop_2977, 
        MagicPropertyType.Prop_2978, 
        MagicPropertyType.Prop_2979, 
        MagicPropertyType.Prop_3170,
        MagicPropertyType.Prop_3171,
        MagicPropertyType.Prop_3172,
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
