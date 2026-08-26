using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

// 再来一次：锁定目标敌人本回合的行动。
// 接下来 X 个回合（X=层数），每回合准备时（玩家回合意图显示）其意图被替换为被锁定的行动。
// 意图替换由 EncoreIntentPatch（PrepareForNextTurn postfix）执行，保证 UI 刷新。
public sealed class EncorePower : PowerModel
{
    private string? _lockedStateId;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public string? LockedStateId => _lockedStateId;

    public bool HasLockedMove() => _lockedStateId != null;

    public void LockMove(string stateId)
    {
        _lockedStateId = stateId;
    }

    // 尝试锁定：优先锁定当前意图；眩晕(STUNNED)时锁定其跟进行动；均无效则不锁，等敌人恢复后由 patch 补锁
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        try
        {
            if (base.Owner?.Monster?.MoveStateMachine == null || _lockedStateId != null)
            {
                return;
            }
            var machine = base.Owner.Monster.MoveStateMachine;
            string id = base.Owner.Monster.NextMove.Id;
            if (!machine.States.ContainsKey(id) && base.Owner.Monster.NextMove.FollowUpStateId != null)
            {
                id = base.Owner.Monster.NextMove.FollowUpStateId;
            }
            if (machine.States.ContainsKey(id))
            {
                _lockedStateId = id;
                MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Encore locked move: {id}");
            }
            else
            {
                MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Encore lock deferred (no valid move): {base.Owner.Monster.NextMove.Id}");
            }
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] Encore lock failed: {ex}");
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
