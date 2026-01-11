using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op151 : MagicOperationBase<Op151>, IOperationBase<Op151>
{
    public override MagicOperationType Type => MagicOperationType.Operation_151;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_48, 
        MagicPropertyType.DelaySecBetweenProjectileCreation, 
        MagicPropertyType.NumProjectilesToSpawn, 
        MagicPropertyType.Prop_93, 
        MagicPropertyType.Prop_102,
        MagicPropertyType.Prop_152
    ];
}
