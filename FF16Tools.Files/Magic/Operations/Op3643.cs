using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op3643 : MagicOperationBase<Op3643>, IOperationBase<Op3643>
{
    public override MagicOperationType Type => MagicOperationType.Operation_3643;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop8_SpeedStart, 
        MagicPropertyType.Prop_32, 
        MagicPropertyType.Prop_ProjectileDuration, 
        MagicPropertyType.Prop_42, 
        MagicPropertyType.Prop_47, 
        MagicPropertyType.Prop_68, 
        MagicPropertyType.Prop_74, 
        MagicPropertyType.Prop_75, 
        MagicPropertyType.Prop_81, 
        MagicPropertyType.Prop_102, 
        MagicPropertyType.Prop_117, 
        MagicPropertyType.Prop_2060, 
        MagicPropertyType.Prop_4107, 
        MagicPropertyType.Prop_4436, 
        MagicPropertyType.Prop_6287, 
        MagicPropertyType.Prop_6459,
        MagicPropertyType.Prop_6831
    ];
}
