using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7732 : MagicOperationBase<Op7732>, IOperationBase<Op7732>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7732;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_47,
        MagicPropertyType.DelaySecBetweenProjectileCreation
    ];
}
