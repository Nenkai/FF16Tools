using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op135 : MagicOperationBase<Op135>, IOperationBase<Op135>
{
    public override MagicOperationType Type => MagicOperationType.Operation_135;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_70_UnkJitterMaxAngleRadXYZ, 
        MagicPropertyType.Prop_114, 
        MagicPropertyType.Prop_134, 
        MagicPropertyType.Prop_136, 
        MagicPropertyType.Prop_1341, 
        MagicPropertyType.Prop_1649, 
        MagicPropertyType.Prop_2153, 
        MagicPropertyType.Prop_2311,
        MagicPropertyType.Prop_2420, 
        MagicPropertyType.Prop_2430, 
        MagicPropertyType.Prop_3764, 
        MagicPropertyType.Prop_4135, 
        MagicPropertyType.Prop_4258, 
        MagicPropertyType.Prop_4259, 
        MagicPropertyType.Prop_4260, 
        MagicPropertyType.Prop_4261, 
        MagicPropertyType.Prop_5642, 
        MagicPropertyType.Prop_5947,
        MagicPropertyType.Prop_6170, 
        MagicPropertyType.Prop_6685,
        MagicPropertyType.Prop_7366, 
        MagicPropertyType.Prop_7711, 
        MagicPropertyType.Prop_7712, 
        MagicPropertyType.Prop_7733,
        MagicPropertyType.Prop_7757
    ];
}
