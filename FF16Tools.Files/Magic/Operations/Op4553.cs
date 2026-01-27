using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4553 : MagicOperationBase<Op4553>, IOperationBase<Op4553>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4553;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Distance
    ];
}
