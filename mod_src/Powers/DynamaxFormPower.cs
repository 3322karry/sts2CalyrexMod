using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Powers;

public sealed class DynamaxFormPower : PowerModel
{
    private int _turnsLeft = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private const float DynamaxScale = 3f;

    // 极巨化：角色精灵放大 3 倍
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        try
        {
            base.Owner?.GetCreatureNode()?.SetDefaultScaleTo(DynamaxScale, 0.5f);
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] DynamaxFormPower scale up: {ex.Message}");
        }
        return Task.CompletedTask;
    }

    // 极巨化结束：还原精灵大小
    public override Task AfterRemoved(Creature oldOwner)
    {
        try
        {
            oldOwner?.GetCreatureNode()?.SetDefaultScaleTo(1f, 0.5f);
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] DynamaxFormPower scale reset: {ex.Message}");
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner)
        {
            return 1m;
        }
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }
        return 3m * base.Amount;
    }

    // 3 个回合（己方回合开始计数）后失效
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player)
        {
            return;
        }
        _turnsLeft--;
        if (_turnsLeft <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}
