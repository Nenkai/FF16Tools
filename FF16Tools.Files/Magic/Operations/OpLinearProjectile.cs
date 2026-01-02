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
        MagicPropertyType.Prop_24
    ];

}
