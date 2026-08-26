using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

public sealed class SlowDownPower : PowerModel
{
    private int _pendingEnergy;
    private CardModel? _lastPlayed;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner?.Player && !cardPlay.Card.EnergyCost.CostsX)
        {
            _lastPlayed = cardPlay.Card;
        }
        await Task.CompletedTask;
    }

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
            if (_lastPlayed != null && base.Owner.Player?.PlayerCombatState != null)
            {
                var copy = base.Owner.Player?.Creature.CombatState?.CloneCard(_lastPlayed);
                if (copy != null)
                {
                    copy.EnergyCost.SetThisCombat(0);
                    copy.AddKeyword(CardKeyword.Exhaust);
                    await CardPileCmd.AddGeneratedCardsToCombat(new[] { copy }, PileType.Hand, base.Owner.Player);
                }
            }
        }
    }
}
