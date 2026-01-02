using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op189 : MagicOperationBase<Op189>, IOperationBase<Op189>
{
    public override MagicOperationType Type => MagicOperationType.Operation_189;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_190, 
        MagicPropertyType.Prop_191, 
        MagicPropertyType.Prop_192,
        MagicPropertyType.Prop_193
    ];
}
