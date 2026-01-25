using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

/// <summary>
/// Same as camera zoom out, but an adjustment can be set.
/// </summary>
public class OpCameraZoomOutWithAdjust : OpOpCameraZoomOutBase<OpCameraZoomOutWithAdjust>, IOperationBase<OpCameraZoomOutWithAdjust>
{
    public override MagicOperationType Type => MagicOperationType.Operation_CameraZoomOutWithAdjust;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
        OpCameraZoomOut.sSupportedProperties.Concat([
            MagicPropertyType.CameraAdjustParamId,
        ])
        .ToHashSet();
}
