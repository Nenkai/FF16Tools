using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op7378 : MagicOperationBase<Op7378>, IOperationBase<Op7378>
{
    public override MagicOperationType Type => MagicOperationType.Operation_7378;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileCreationOpGroupId, 
        MagicPropertyType.Prop_30, 
        MagicPropertyType.DelaySecBetweenProjectileCreation, 
        MagicPropertyType.Prop_76, 
        MagicPropertyType.ProjectileMaxDistance, 
        MagicPropertyType.Prop_7379, 
        MagicPropertyType.Prop_7380, 
        MagicPropertyType.Prop_7411,
        MagicPropertyType.Prop_7412
    ];
}
