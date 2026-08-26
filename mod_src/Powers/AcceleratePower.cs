using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Powers;

public sealed class AcceleratePower : PowerModel
{
    private int _pendingEnergy;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (base.Owner?.Player == null || card.Owner != base.Owner.Player)
        {
            return;
        }
        _pendingEnergy += amount;
        while (_pendingEnergy >= 2)
        {
            _pendingEnergy -= 2;
            await PlayerCmd.GainEnergy(1m, base.Owner.Player);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner, System.Math.Max(1m, base.Amount), base.Owner, null);
        }
    }
}
