using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op4274 : MagicOperationBase<Op4274>, IOperationBase<Op4274>
{
    public override MagicOperationType Type => MagicOperationType.Operation_4274;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3113,
        MagicPropertyType.Prop_3114, 
        MagicPropertyType.Prop_3115, 
        MagicPropertyType.Prop_3116, 
        MagicPropertyType.Prop_3694, 
        MagicPropertyType.Prop_4275,
        MagicPropertyType.Prop_4276
    ];
}
