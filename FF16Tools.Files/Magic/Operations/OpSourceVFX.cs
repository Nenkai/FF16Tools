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
        MagicPropertyType.VFXScale, 
        MagicPropertyType.VFX_XYZOffset,
        MagicPropertyType.Prop_34, 
        MagicPropertyType.ActorVFX_XYZOffset, 
        MagicPropertyType.ActorVFXAudioId, 
        MagicPropertyType.Prop_90_UseProp91, 
        MagicPropertyType.Prop_91, 
        MagicPropertyType.Prop_2359, 
        MagicPropertyType.Prop_4101,
        MagicPropertyType.Prop_4102,
        MagicPropertyType.Prop_6056,
    ];
}
