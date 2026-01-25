using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op2325 : MagicOperationBase<Op2325>, IOperationBase<Op2325>
{
    public override MagicOperationType Type => MagicOperationType.Operation_2325;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.ProjectileSpeedStart,
        MagicPropertyType.VFX_XYZOffset,
        MagicPropertyType.ProjectileDuration,
        MagicPropertyType.HeightUnk,
        MagicPropertyType.UnkMax,
        MagicPropertyType.NumProjectilesToSpawn,
        MagicPropertyType.Prop_93,
        MagicPropertyType.Prop_147,
        MagicPropertyType.Prop_148,
        MagicPropertyType.Prop_149,
        MagicPropertyType.Prop_150,
        MagicPropertyType.Prop_2413_UnkRateForProp47,
        MagicPropertyType.Prop_2414_UnkMaxForProp47,
        MagicPropertyType.Prop_3848,
        MagicPropertyType.Prop_3907
    ];
}
