using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3433 : Op3433Base<Op3433>, IOperationBase<Op3433>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3433;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } = BaseProperties;
}

public abstract class Op3433Base<T> : MagicOperationBase<T>
    where T : Op3433Base<T>, IOperationBase<T>
{
    protected static HashSet<MagicPropertyType> BaseProperties { get; set; } =
    [
        MagicPropertyType.Prop_3434,
        MagicPropertyType.Prop_3440,
        MagicPropertyType.Prop_3441,
        MagicPropertyType.Prop_3608,
        MagicPropertyType.Prop_3609,
        MagicPropertyType.Prop_3691,
        MagicPropertyType.Prop_3692,
        MagicPropertyType.Prop_3693,
        MagicPropertyType.Prop_5142,
        MagicPropertyType.Prop_5143,
        MagicPropertyType.Prop_5685,
        MagicPropertyType.Prop_5799
    ];
}