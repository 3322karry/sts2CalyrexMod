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

// 再来一次：锁定目标敌人本回合的行动。
// 接下来 X 个回合（X=层数），每回合开始时（玩家回合，意图显示时）将其意图替换为被锁定的行动。
public sealed class EncorePower : PowerModel
{
    private string? _lockedStateId;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 施放时记录：锁定敌人当前意图（NextMove）对应的状态
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

    // 玩家回合开始：敌人意图正显示，此时替换为锁定行动（含 UI 刷新）
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player)
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
            var lockedState = machine.States[_lockedStateId] as MoveState;
            if (lockedState == null)
            {
                return;
            }
            // 替换意图（含 UI 刷新）——意图在玩家回合显示，替换后玩家可见
            base.Owner.Monster.SetMoveImmediate(lockedState, forceTransition: true);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Encore intent replaced: {_lockedStateId}");
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] Encore intent replace failed: {ex}");
        }
        await Task.CompletedTask;
    }

    // 敌人回合结束：掉 1 层（持续回合数）
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
        {
            return;
        }
        await PowerCmd.TickDownDuration(this);
        await Task.CompletedTask;
    }
}
