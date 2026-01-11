using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op141 : MagicOperationBase<Op141>, IOperationBase<Op141>
{
    public override MagicOperationType Type => MagicOperationType.Operation_141;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.NumProjectilesToSpawn, 
        MagicPropertyType.OnFinishedOpGroupId, 
        MagicPropertyType.Prop_142,
        MagicPropertyType.Prop_143
    ];
}
