using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Powers;

public sealed class EternalWhinny : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 骑马状态下：每有一个敌人死亡 +2 力量（单敌战斗敌人死亡即战斗结束，无收益）
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.PetOwner != null || creature.Player != null)
        {
            return;
        }

        if (base.Owner == null || base.Owner.CombatState == null)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, 2m, base.Owner, null);
    }
}
