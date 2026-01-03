using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op2493 : MagicOperationBase<Op2493>, IOperationBase<Op2493>
{
    public override MagicOperationType Type => MagicOperationType.Operation_2493;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3, 
        MagicPropertyType.Prop8_SpeedStart, 
        MagicPropertyType.Prop_187, 
        MagicPropertyType.Prop_2351,
        MagicPropertyType.Prop_2430, 
        MagicPropertyType.Prop_2593, 
        MagicPropertyType.Prop_3145, 
        MagicPropertyType.Prop_7691, 
        MagicPropertyType.Prop_7734,
        MagicPropertyType.Prop_7735
    ];
}
