using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7808 : MagicOperationBase<Op7808>, IOperationBase<Op7808>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7808;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ, 
        MagicPropertyType.Prop_4102,
        MagicPropertyType.Prop_7809
    ];
}
