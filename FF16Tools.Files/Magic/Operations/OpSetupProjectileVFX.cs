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
        MagicPropertyType.Prop_VFXAudioId, 
        MagicPropertyType.Prop_28, 
        MagicPropertyType.Prop_29, 
        MagicPropertyType.Prop_30, 
        MagicPropertyType.Prop_VFXScale, 
        MagicPropertyType.Prop_33, 
        MagicPropertyType.Prop_34, 
        MagicPropertyType.Prop_81, 
        MagicPropertyType.Prop_1231, 
        MagicPropertyType.Prop_2359, 
        MagicPropertyType.Prop_2798, 
        MagicPropertyType.Prop_2799, 
        MagicPropertyType.Prop_3772, 
        MagicPropertyType.Prop_4101, 
        MagicPropertyType.Prop_4102, 
        MagicPropertyType.Prop_4243, 
        MagicPropertyType.Prop_4244, 
        MagicPropertyType.Prop_5959, 
        MagicPropertyType.Prop_6056, 
        MagicPropertyType.Prop_7791,
        MagicPropertyType.Prop_7813
    ];

}
