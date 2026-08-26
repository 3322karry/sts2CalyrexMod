using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace CalyrexMod.Powers;

// 再来一次：锁定目标敌人本回合的行动，未来 X 回合（含本回合后的下个回合起）
// 每回合结束时重复执行该行动，并替换其意图为该行动。
public sealed class EncorePower : PowerModel
{
    private string? _lockedStateId;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 施放时记录：锁定的行动 = 敌人当前意图（NextMove）对应的状态
    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await Task.CompletedTask;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        try
        {
            if (base.Owner?.Monster?.MoveStateMachine != null && _lockedStateId == null)
            {
                string id = base.Owner.Monster.NextMove.Id;
                if (base.Owner.Monster.MoveStateMachine.States.ContainsKey(id))
                {
                    _lockedStateId = id;
                    MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Encore locked move: {id}");
                }
            }
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] Encore lock failed: {ex}");
        }
        await Task.CompletedTask;
    }

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
            var machine = base.Owner.Monster.MoveStateMachine;
            if (machine == null || _lockedStateId == null || !machine.States.ContainsKey(_lockedStateId))
            {
                return;
            }
            // 替换意图：强制状态机进入锁定行动（替换下回合意图显示）
            machine.ForceCurrentState(machine.States[_lockedStateId]);
            // 重复执行该行动
            await base.Owner.Monster.PerformMove();
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] EncorePower repeat move failed: {ex}");
        }
        await PowerCmd.TickDownDuration(this);
    }
}
