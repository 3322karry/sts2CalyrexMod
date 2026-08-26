using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

// 目标敌人每回合结束后重复一次本回合行动，持续 X 回合
public sealed class EncorePower : PowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
        {
            return;
        }
        if (base.Owner == null || !base.Owner.IsAlive || base.Owner.Monster == null)
        {
            return;
        }
        try
        {
            await base.Owner.Monster.PerformMove();
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] EncorePower repeat move failed: {ex}");
        }
        await PowerCmd.TickDownDuration(this);
    }
}
