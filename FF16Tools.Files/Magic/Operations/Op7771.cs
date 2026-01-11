using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7771 : MagicOperationBase<Op7771>, IOperationBase<Op7771>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7771;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileYOffset, 
        MagicPropertyType.Prop_117, 
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ,
        MagicPropertyType.Prop_4102
    ];
}
