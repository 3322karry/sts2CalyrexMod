using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

public sealed class IngrainPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 获得丰饶时抽一张
    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (power is Abundance && base.Owner?.Player?.PlayerCombatState != null)
        {
            await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), base.Amount, base.Owner.Player);
        }
    }

    // 马获得喂养（血量增加）时抽一张
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (delta <= 0m || creature.PetOwner?.Creature != base.Owner || base.Owner?.Player?.PlayerCombatState == null)
        {
            return;
        }
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), base.Amount, base.Owner.Player);
    }
}
