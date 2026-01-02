using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op87 : MagicOperationBase<Op87>, IOperationBase<Op87>
{
    public override MagicOperationType Type => MagicOperationType.Operation_87_SourceVFX;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_VFXScale, 
        MagicPropertyType.Prop_34, 
        MagicPropertyType.Prop_53, 
        MagicPropertyType.Prop_89, 
        MagicPropertyType.Prop_90, 
        MagicPropertyType.Prop_91, 
        MagicPropertyType.Prop_1954, 
        MagicPropertyType.Prop_2359, 
        MagicPropertyType.Prop_4101,
        MagicPropertyType.Prop_4102
    ];
}
