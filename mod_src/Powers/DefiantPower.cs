using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

// 不服输：每次被施加负面效果时，加 1 点力量
public sealed class DefiantPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != base.Owner)
        {
            return;
        }
        if (power.Type != PowerType.Debuff || amount <= 0m)
        {
            return;
        }
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, base.Owner, null);
    }
}
