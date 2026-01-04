using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Magic.Operations;

public class Op6473 : MagicOperationBase<Op6473>, IOperationBase<Op6473>
{
    public override MagicOperationType Type => MagicOperationType.Operation_6473;
    public static HashSet<MagicPropertyType> sSupportedProperties { get; set; } =
    [
        MagicPropertyType.MinSkillUpgradeLevelForAttack_5274_5275, 
        MagicPropertyType.Prop_6474,
        MagicPropertyType.Prop_6475
    ];
}
