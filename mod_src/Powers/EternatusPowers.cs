using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Powers;

// 无极巨化：阶段1 buff（视觉切换由怪物处理）
public sealed class EternamaxPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}

// 混乱：破盾时获得 2 虚弱 2 易伤 2 脆弱
public sealed class PanicPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterBlockBroken(Creature creature)
    {
        if (creature != base.Owner)
        {
            return;
        }
        var p = base.Owner.CombatState?.Players.FirstOrDefault();
        if (p == null)
        {
            return;
        }
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), p.Creature, 2m, base.Owner, null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), p.Creature, 2m, base.Owner, null);
        await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), p.Creature, 2m, base.Owner, null);
    }
}

// 震慑：本回合玩家只能打出一张攻击牌
public sealed class AwePower : PowerModel
{
    private int _attacksPlayedThisTurn;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldPlay(CardModel card, AutoPlayType _)
    {
        if (card.Owner.Creature != base.Owner)
        {
            return true;
        }
        if (card.Type != CardType.Attack)
        {
            return true;
        }
        return _attacksPlayedThisTurn < 1;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay.Card.Type == CardType.Attack)
        {
            _attacksPlayedThisTurn++;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _attacksPlayedThisTurn = 0;
        }
        await Task.CompletedTask;
    }
}
