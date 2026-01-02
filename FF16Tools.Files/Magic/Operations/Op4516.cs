using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4516 : MagicOperationBase<Op4516>, IOperationBase<Op4516>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4516;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_147
    ];
}
