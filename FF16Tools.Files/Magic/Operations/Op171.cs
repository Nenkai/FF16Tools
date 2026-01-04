using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op171 : MagicOperationBase<Op171>, IOperationBase<Op171>
{
    public override MagicOperationType Type => MagicOperationType.Operation_171;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart, 
        MagicPropertyType.ProjectileSpeedIncreaseRate, 
        MagicPropertyType.ProjectileSpeedStartRangeRand, 
        MagicPropertyType.ProjectileSpeedMax, 
        MagicPropertyType.ProjectileDirectionAngles, 
        MagicPropertyType.Prop_24, 
        MagicPropertyType.Prop_145,
        MagicPropertyType.Prop_172, 
        MagicPropertyType.Prop_173, 
        MagicPropertyType.Prop_174, 
        MagicPropertyType.Prop_175, 
        MagicPropertyType.Prop_176, 
        MagicPropertyType.Prop_177, 
        MagicPropertyType.Prop_178, 
        MagicPropertyType.Prop_179, 
        MagicPropertyType.Prop_180, 
        MagicPropertyType.Prop_181, 
        MagicPropertyType.Prop_1793, 
        MagicPropertyType.Prop_1794, 
        MagicPropertyType.Prop_1796, 
        MagicPropertyType.Prop_2028, 
        MagicPropertyType.Prop_4003, 
        MagicPropertyType.Prop_4060, 
        MagicPropertyType.Prop_4128_UnkTargetType,
        MagicPropertyType.Prop_4296
    ];
}
