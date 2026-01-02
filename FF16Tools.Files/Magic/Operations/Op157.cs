using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op157 : MagicOperationBase<Op157>, IOperationBase<Op157>
{
    public override MagicOperationType Type => MagicOperationType.Operation_157;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop8_SpeedStart, 
        MagicPropertyType.Prop_81, 
        MagicPropertyType.Prop_147, 
        MagicPropertyType.Prop_148, 
        MagicPropertyType.Prop_149, 
        MagicPropertyType.Prop_158,
        MagicPropertyType.Prop_159
    ];
}
