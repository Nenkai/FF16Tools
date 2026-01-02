using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op144 : MagicOperationBase<Op144>, IOperationBase<Op144>
{
    public override MagicOperationType Type => MagicOperationType.Operation_144;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop14_UnkMaxAngleRad, 
        MagicPropertyType.Prop_23, 
        MagicPropertyType.Prop_26, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_74, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_81,
        MagicPropertyType.Prop_145
    ];
}
