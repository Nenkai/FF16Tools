using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op139 : Op139Base<Op139>, IOperationBase<Op139>
{
    public override MagicOperationType Type => MagicOperationType.Operation_139;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } = BaseProperties;
}

public abstract class Op139Base<T> : MagicOperationBase<T>
    where T : Op139Base<T>, IOperationBase<T>
{
    protected static HashSet<MagicPropertyType> BaseProperties { get; set; } =
    [
        MagicPropertyType.ProjectileDirectionAngles,
        MagicPropertyType.Distance,
        MagicPropertyType.DistanceIncreaseRate,
        MagicPropertyType.DistanceMax,
        MagicPropertyType.Prop_114,
        MagicPropertyType.Prop_140,
        MagicPropertyType.Prop_3349,
        MagicPropertyType.Prop_4128_UnkTargetType
    ];
}
