using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op2325 : MagicOperationBase<Op2325>, IOperationBase<Op2325>
{
    public override MagicOperationType Type => MagicOperationType.Operation_2325;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop8_SpeedStart,
        MagicPropertyType.Prop_32,
        MagicPropertyType.Prop_ProjectileDuration,
        MagicPropertyType.Prop_47,
        MagicPropertyType.Prop_48,
        MagicPropertyType.Prop_75,
        MagicPropertyType.Prop_93,
        MagicPropertyType.Prop_147,
        MagicPropertyType.Prop_148,
        MagicPropertyType.Prop_149,
        MagicPropertyType.Prop_150,
        MagicPropertyType.Prop_2413,
        MagicPropertyType.Prop_2414,
        MagicPropertyType.Prop_3848,
        MagicPropertyType.Prop_3907
    ];
}
