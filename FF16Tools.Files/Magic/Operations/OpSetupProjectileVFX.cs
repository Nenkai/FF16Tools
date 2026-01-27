using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class OpSetupProjectileVFX : MagicOperationBase<OpSetupProjectileVFX>, IOperationBase<OpSetupProjectileVFX>
{
    public override MagicOperationType Type => MagicOperationType.Operation_25_SetupProjectileVFX;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.Prop_24,                         // Default 0
        MagicPropertyType.MainVFXAudioId,                  // Default 0
        MagicPropertyType.IsManualPlacement,
        MagicPropertyType.Prop_29,                         // Default false
        MagicPropertyType.CancelOnProjectileEvent,         // Default true (flag 1)
        MagicPropertyType.VFXScale,                        // Default 1.0
        MagicPropertyType.VFX_XYZOffset,                   // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Prop_33,                         // Default 0
        MagicPropertyType.Prop_34,                         // Default false
        MagicPropertyType.ActorEidForProjectileSource,     // Default 0
        MagicPropertyType.Prop_1231_VFXAudioIdElem1,       // Default 0
        MagicPropertyType.Prop_1232_VFXAudioIdElem2,       // Default 0
        MagicPropertyType.Prop_2359,                       // Default false
        MagicPropertyType.VFXUnk1_Elem1,                   // Default -1?
        MagicPropertyType.VFXUnk1_Elem2,                   // Default -1?
        MagicPropertyType.Prop_3772,                       // Default 0
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ, // Default 1.0
        MagicPropertyType.Prop_4102,                       // Default 0.2
        MagicPropertyType.VFXUnk2_Elem1,                   // Default 0
        MagicPropertyType.VFXUnk2_Elem2,                   // Default 0
        MagicPropertyType.VFXColorUnk,                     // Default <0.0, 0.0, 0.0>
        MagicPropertyType.Prop_6056,                       // Default false
        MagicPropertyType.VFXUnk2Alt_Elem1,                // Default 0 (maps to VFXUnk2_Elem1)
        MagicPropertyType.VFXUnk2Alt_Elem2,                // Default 0
        MagicPropertyType.Prop_7813                        // Default false
    ];

}
