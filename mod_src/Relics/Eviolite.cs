using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Relics;

public sealed class Eviolite : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // 未骑马时，回合开始获得 6 格挡
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || base.Owner == null)
        {
            return;
        }
        bool mounted = base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier || p is MountedSpectrier);
        if (!mounted)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, 6m, ValueProp.Unpowered, null);
        }
    }
}
