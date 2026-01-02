using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op990 : MagicOperationBase<Op990>, IOperationBase<Op990>
{
    public override MagicOperationType Type => MagicOperationType.Operation_990;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_104, 
        MagicPropertyType.Prop_105, 
        MagicPropertyType.Prop_147, 
        MagicPropertyType.Prop_159, 
        MagicPropertyType.Prop_994,
        MagicPropertyType.Prop_995
    ];
}
