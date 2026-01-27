using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op5133 : MagicOperationBase<Op5133>, IOperationBase<Op5133>
{
    public override MagicOperationType Type => MagicOperationType.Operation_5133;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.IsManualPlacement, 
        MagicPropertyType.CancelOnProjectileEvent, 
        MagicPropertyType.Prop_46, 
        MagicPropertyType.UnkMax, 
        MagicPropertyType.CameraFCurveId,
        MagicPropertyType.CameraFCurveId2,
        MagicPropertyType.Prop_6135
    ];
}
