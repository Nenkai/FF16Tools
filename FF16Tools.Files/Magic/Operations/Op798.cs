using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op798 : MagicOperationBase<Op798>, IOperationBase<Op798>
{
    public override MagicOperationType Type => MagicOperationType.Operation_798;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_110,
        MagicPropertyType.Prop_188
    ];
}
