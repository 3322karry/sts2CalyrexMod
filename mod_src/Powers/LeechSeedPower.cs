using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Powers;

public sealed class LeechSeedPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || base.Owner == null)
        {
            return;
        }
        // 使一名敌人失去 6 血，你回复 3 血
        var target = combatState.Enemies.FirstOrDefault((Creature e) => e.IsAlive);
        if (target != null)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, 6m * base.Amount, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner, null);
            await CreatureCmd.Heal(base.Owner, 3m * base.Amount, playAnim: false);
        }
    }
}
