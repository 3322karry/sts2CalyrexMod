using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

// 特性膏药效果：第一回合前 N 张能力牌免费打出
public sealed class AbilityCapsulePower : PowerModel
{
    private int _freeLeft;
    private bool _isFirstTurn = true;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeCombatStart()
    {
        _freeLeft = base.Amount;
        _isFirstTurn = true;
        await Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player && !_isFirstTurn)
        {
            _isFirstTurn = false;
            _freeLeft = 0;
        }
        await Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != base.Owner)
        {
            return false;
        }
        if (card.Type != CardType.Power)
        {
            return false;
        }
        if (_freeLeft <= 0)
        {
            return false;
        }
        modifiedCost = 0m;
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay.Card.Type == CardType.Power && _freeLeft > 0)
        {
            _freeLeft--;
        }
        await Task.CompletedTask;
    }
}
