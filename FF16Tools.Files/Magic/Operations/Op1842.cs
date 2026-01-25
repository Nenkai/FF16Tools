using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op1842 : OpOpCameraZoomOutBase<Op1842>, IOperationBase<Op1842>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1842;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } = 
        OpCameraZoomOut.sSupportedProperties.Concat([
            MagicPropertyType.Prop_2575_AttackParamId
        ])
        .ToHashSet();
}
