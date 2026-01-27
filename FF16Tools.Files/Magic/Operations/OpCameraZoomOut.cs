using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpCameraZoomOut : OpOpCameraZoomOutBase<OpCameraZoomOut>, IOperationBase<OpCameraZoomOut>
{
    public override MagicOperationType Type => MagicOperationType.Operation_CameraZoomOut;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } = BaseProperties;
}

public abstract class OpOpCameraZoomOutBase<T> : MagicOperationBase<T>
    where T : OpOpCameraZoomOutBase<T>, IOperationBase<T>
{
    protected static HashSet<MagicPropertyType> BaseProperties { get; set; } =
    [
        MagicPropertyType.VFX_XYZOffset,                         // Default 0
        MagicPropertyType.Distance,                              // Default 0
        MagicPropertyType.DistanceIncreaseRate,                  // Default 0
        MagicPropertyType.DistanceMax,                           // Default 0
        MagicPropertyType.CollisionShapeType,                    // Default 0
        MagicPropertyType.Prop_46,                               // Default 0
        MagicPropertyType.HeightUnk,                             // Default 0
        MagicPropertyType.UnkMax,                                // Default 0
        MagicPropertyType.DelaySecond,                           // Default 0
        MagicPropertyType.Prop_114,                              // Default 0
        MagicPropertyType.Prop_2430                              // Default 0
    ];
}
