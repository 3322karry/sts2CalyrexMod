using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Powers;

public sealed class LeechSeedPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 敌人回合结束：寄生种子对携带者生效（失去 8 血，你回复 4 血）
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy || base.Owner == null || !base.Owner.IsAlive)
        {
            return;
        }
        var player = base.Owner.CombatState?.Players.FirstOrDefault((Player p) => p.Creature != null)?.Creature;
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, 8m * base.Amount, ValueProp.Unblockable | ValueProp.Unpowered, player, null);
        if (player != null)
        {
            await CreatureCmd.Heal(player, 4m * base.Amount, playAnim: false);
        }
    }
}
