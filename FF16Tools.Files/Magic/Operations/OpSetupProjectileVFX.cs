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
        MagicPropertyType.Prop_24, 
        MagicPropertyType.MainVFXAudioId, 
        MagicPropertyType.Prop_28, 
        MagicPropertyType.Prop_29, 
        MagicPropertyType.Prop_30, 
        MagicPropertyType.VFXScale, 
        MagicPropertyType.VFX_XYZOffset,
        MagicPropertyType.Prop_33, 
        MagicPropertyType.Prop_34, 
        MagicPropertyType.ActorEidForProjectileSource, 
        MagicPropertyType.Prop_1231_VFXAudioIdElem1,
        MagicPropertyType.Prop_1232_VFXAudioIdElem2,
        MagicPropertyType.Prop_2359, 
        MagicPropertyType.VFXUnk1_Elem1, 
        MagicPropertyType.VFXUnk1_Elem2, 
        MagicPropertyType.Prop_3772, 
        MagicPropertyType.Prop_4101_UnkJitterMaxAngleRadZ, 
        MagicPropertyType.Prop_4102, 
        MagicPropertyType.VFXUnk2_Elem1, 
        MagicPropertyType.VFXUnk2_Elem2, 
        MagicPropertyType.VFXColorUnk, 
        MagicPropertyType.Prop_6056, 
        MagicPropertyType.VFXUnk2Alt_Elem1,
        MagicPropertyType.VFXUnk2Alt_Elem2,
        MagicPropertyType.Prop_7813
    ];

}
