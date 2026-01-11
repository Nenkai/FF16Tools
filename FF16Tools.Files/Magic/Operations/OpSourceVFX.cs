using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpSourceVFX : MagicOperationBase<OpSourceVFX>, IOperationBase<OpSourceVFX>
{
    public override MagicOperationType Type => MagicOperationType.Operation_87_SourceVFX;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.VFXScale,                        // Default 1.0
        MagicPropertyType.VFX_XYZOffset,                   // Default 0
        MagicPropertyType.Prop_34,                         // Default false
        MagicPropertyType.Projectile2XYZOffset,            // Default <0.0, 0.0, 0.0>
        MagicPropertyType.ActorVFXAudioId,                 // Default 0
        MagicPropertyType.Prop_90_UseProp91,               // Default false
        MagicPropertyType.Prop_91,                         // Default 0
        MagicPropertyType.Prop_2359,                       // Default 0
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ, // Default 3.0
        MagicPropertyType.Prop_4102,                       // Default 0.2
        MagicPropertyType.Prop_6056,                       // Default 0
    ];
}
