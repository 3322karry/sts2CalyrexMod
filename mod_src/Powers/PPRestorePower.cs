using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

public sealed class PPRestorePower : PowerModel
{
    private bool _usedThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 每回合第一次使用 X 费牌时，回复能量
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.Owner?.Player == null || cardPlay.Card.Owner != base.Owner.Player)
        {
            return;
        }
        if (!cardPlay.Card.EnergyCost.CostsX || _usedThisTurn)
        {
            return;
        }
        _usedThisTurn = true;
        await PlayerCmd.GainEnergy(System.Math.Max(1m, base.Amount), base.Owner.Player);
    }

    // 回合开始重置
    public override async Task AfterSideTurnStart(MegaCrit.Sts2.Core.Combat.CombatSide side, System.Collections.Generic.IReadOnlyList<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants, MegaCrit.Sts2.Core.Combat.ICombatState combatState)
    {
        if (side == MegaCrit.Sts2.Core.Combat.CombatSide.Player)
        {
            _usedThisTurn = false;
        }
        await Task.CompletedTask;
    }
}
