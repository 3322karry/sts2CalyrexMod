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

    // 玩家受到的攻击伤害：减为 0（马承受），记录原伤害
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return amount;
        }
        _pendingDamage = amount;
        return 0m;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return;
        }
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
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, remaining, ValueProp.Unblockable | ValueProp.Unpowered, dealer, cardSource);
        }
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
}
