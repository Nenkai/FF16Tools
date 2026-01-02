using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3941 : MagicOperationBase<Op3941>, IOperationBase<Op3941>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3941;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_46,
        MagicPropertyType.Prop_4165
    ];
}
