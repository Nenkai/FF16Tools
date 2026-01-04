using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op128 : MagicOperationBase<Op128>, IOperationBase<Op128>
{
    public override MagicOperationType Type => MagicOperationType.Operation_128;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.Prop_15, 
        MagicPropertyType.Prop_47, 
        MagicPropertyType.Prop_69_TargetType, 
        MagicPropertyType.Prop_129, 
        MagicPropertyType.Prop_130, 
        MagicPropertyType.Prop_131, 
        MagicPropertyType.Prop_132, 
        MagicPropertyType.Prop_3907, 
        MagicPropertyType.Prop_4277, 
        MagicPropertyType.Prop_6723,
        MagicPropertyType.Prop_6781, 
        MagicPropertyType.Prop_6782, 
        MagicPropertyType.Prop_7046,
        MagicPropertyType.Prop_7047
    ];
}
