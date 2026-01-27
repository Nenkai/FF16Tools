using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpCameraShakeCurve : MagicOperationBase<OpCameraShakeCurve>, IOperationBase<OpCameraShakeCurve>
{
    public override MagicOperationType Type => MagicOperationType.Operation_CameraShakeCurve;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.IsManualPlacement,       // Default false
        MagicPropertyType.CancelOnProjectileEvent, // Default true - for CameraShake event, if magic event is 1, 4 or 5, do not process
        MagicPropertyType.Distance,                // Default 10.0
        MagicPropertyType.CameraFCurveId,          // Default 0 (none)
        MagicPropertyType.CameraFCurveId2,         // Default 0 (none)
        MagicPropertyType.CameraShakeDelay,        // Default 0
        MagicPropertyType.CameraShakeTypeUnk,      // Default 0
        MagicPropertyType.CameraShakeStart,        // Default 0
        MagicPropertyType.CameraShakeIncreaseRate, // Default 0
        MagicPropertyType.CameraShakeMax           // Default 0


        // To get delay working:
        // - set delay
        // - Prop28 must be false
        // - Curve id must be set
        // - DistanceStart is actually max distance

        // Otherwise shake happens immediately, CameraShakeStart/IncreaseRate/Max can be used
    ];
}
