using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpLinearProjectile : MagicOperationBase<OpLinearProjectile>, IOperationBase<OpLinearProjectile>
{
    public override MagicOperationType Type => MagicOperationType.Operation_1_LinearProjectile;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_3, 
        MagicPropertyType.Prop_4, 
        MagicPropertyType.Prop_5, 
        MagicPropertyType.Prop_6, 
        MagicPropertyType.Prop_7, 
        MagicPropertyType.Prop8_SpeedStart, 
        MagicPropertyType.Prop9_SpeedIncreaseRate, 
        MagicPropertyType.Prop10_SpeedStatRangeRand,
        MagicPropertyType.Prop11_SpeedMax, 
        MagicPropertyType.Prop_12, 
        MagicPropertyType.Prop13_NoTrackingTarget, 
        MagicPropertyType.Prop14_UnkMaxAngleRad, 
        MagicPropertyType.Prop_15, 
        MagicPropertyType.Prop_16, 
        MagicPropertyType.Prop_17, 
        MagicPropertyType.Prop_18, 
        MagicPropertyType.Prop_19, 
        MagicPropertyType.Prop_20, 
        MagicPropertyType.Prop_21, 
        MagicPropertyType.Prop22_VerticalAngleDegreesOffset,
        MagicPropertyType.Prop_23, 
        MagicPropertyType.Prop_24,
        MagicPropertyType.Prop_1269, 
        MagicPropertyType.Prop_2211, 
        MagicPropertyType.Prop_2286, 
        MagicPropertyType.Prop_2351, 
        MagicPropertyType.Prop_2368, 
        MagicPropertyType.Prop_2856, 
        MagicPropertyType.Prop_3513, 
        MagicPropertyType.Prop_3514, 
        MagicPropertyType.Prop_3544,
        MagicPropertyType.Prop_3605, 
        MagicPropertyType.Prop_3606, 
        MagicPropertyType.Prop_3722, 
        MagicPropertyType.Prop_4094, 
        MagicPropertyType.Prop_4101, 
        MagicPropertyType.Prop_4102, 
        MagicPropertyType.Prop_4128, 
        MagicPropertyType.Prop_5644, 
        MagicPropertyType.Prop_6775,
        MagicPropertyType.Prop_6776, 
        MagicPropertyType.Prop_6825, 
        MagicPropertyType.Prop_6826,
        MagicPropertyType.Prop_6988
    ];

}
