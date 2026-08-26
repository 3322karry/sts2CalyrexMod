using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Linq;

namespace CalyrexMod.Powers;

public sealed class SoulHeartPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 每有单位死亡（包括马），抽一张牌 + 加一费
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (base.Owner?.Player?.PlayerCombatState == null)
        {
            return;
        }
        // 骑马合体的马不算死亡，不触发
        if (creature.Powers.Any((PowerModel p) => p is MountMergePower))
        {
            return;
        }
        await CardPileCmd.Draw(choiceContext, 2m, base.Owner.Player);
        await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
    }
}
