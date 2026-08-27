using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Monsters;

namespace CalyrexMod.Powers;

// 马匹守护（玩家侧）：蕾冠王受到的攻击伤害由马承受。
// 白马先承受；白马被击杀后，剩余伤害立即转给黑马；黑马也死则玩家承受。
public sealed class SteedGuardPassive : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldPlayVfx => false;

    private decimal _pendingDamage;
    private bool _isTransferring;

    // 玩家受到的攻击伤害：减为 0（马承受），记录原伤害
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || _isTransferring)
        {
            return amount;
        }
        // 无活马时直接承受（防止递归转移）
        var player = base.Owner.Player;
        bool anySteed = player?.PlayerCombatState?.GetPet<Glastrier>() is Creature g && g.IsAlive
            || player?.PlayerCombatState?.GetPet<Spectrier>() is Creature sp && sp.IsAlive;
        if (!anySteed)
        {
            return amount;
        }
        try
        {
            _pendingDamage = amount;
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod-SG] ModifyHpLost: {amount} dmg -> pending (dealer={dealer?.LogName})");
        }
        catch (System.Exception)
        {
        }
        return 0m;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod-SG] AfterDamageReceived: target={target?.LogName} pending={_pendingDamage} transferring={_isTransferring}");
        if (target != base.Owner || _isTransferring)
        {
            return;
        }
        _isTransferring = true;
        try
        {
            await Transfer(choiceContext, result, props, dealer, cardSource);
        }
        finally
        {
            _isTransferring = false;
        }
    }

    private async Task Transfer(PlayerChoiceContext choiceContext, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        decimal remaining = _pendingDamage;
        _pendingDamage = 0m;
        if (remaining <= 0m)
        {
            return;
        }
        var player = base.Owner.Player;
        if (player?.PlayerCombatState == null)
        {
            return;
        }
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] SteedGuardPassive: transferring {remaining} dmg to steeds");
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod-SG] Transfer START {remaining}");

        // 白马先承受，溢出转黑马，再溢出转玩家
        var glastrier = player.PlayerCombatState.GetPet<Glastrier>();
        if (glastrier != null && glastrier.IsAlive)
        {
            remaining = await HitSteed(choiceContext, glastrier, remaining, dealer, cardSource);
        }
        if (remaining > 0m)
        {
            var spectrier = player.PlayerCombatState.GetPet<Spectrier>();
            if (spectrier != null && spectrier.IsAlive)
            {
                remaining = await HitSteed(choiceContext, spectrier, remaining, dealer, cardSource);
            }
        }
        if (remaining > 0m)
        {
            // 两马都死：玩家直接承受（绕开转移，避免递归）
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] SteedGuardPassive: no steeds alive, player takes {remaining}");
            base.Owner.LoseHpInternal(remaining, ValueProp.Unblockable | ValueProp.Unpowered);
        }
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod-SG] Transfer END remaining={remaining}");
    }

    // 打马并返回溢出伤害
    private async Task<decimal> HitSteed(PlayerChoiceContext choiceContext, Creature steed, decimal dmg, Creature? dealer, CardModel? cardSource)
    {
        var results = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), steed, dmg, ValueProp.Unblockable | ValueProp.Unpowered, dealer, cardSource);
        decimal dealt = results.Sum((DamageResult r) => r.TotalDamage);
        return dmg - dealt;
    }
}

// 宠物马可被打标记（ShouldAllowHitting 控制马能否作为目标）
public sealed class SteedTargetablePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldAllowHitting(Creature creature)
    {
        return creature.IsAlive;
    }

    // 马死亡后留在场上（死体），喂养时 GainMaxHp 直接复活；
    // 骑马合体（Kill force + MountMergePower）的死亡正常移除，避免卸载时槽位混乱
    // 注意：hook 会对所有死亡的生物调用，非自己（马）时必须返回 true（不干预移除）
    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        if (creature != base.Owner)
        {
            return true;
        }
        return creature.Powers.Any((PowerModel p) => p is MountMergePower);
    }
}
