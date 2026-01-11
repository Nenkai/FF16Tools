using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op6453 : MagicOperationBase<Op6453>, IOperationBase<Op6453>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6453;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileYOffset, 
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ,
        MagicPropertyType.Prop_4102
    ];
}
