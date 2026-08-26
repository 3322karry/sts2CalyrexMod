using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Monsters;

namespace CalyrexMod.Powers;

// 马匹守护（玩家侧）：蕾冠王受到的攻击伤害由马承受。
// 用 ModifyUnblockedDamageTarget（412694 确定调用）把目标改为活马。
public sealed class SteedGuardPassive : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldPlayVfx => false;

    public override Creature ModifyUnblockedDamageTarget(Creature target, decimal amount, ValueProp props, Creature? dealer)
    {
        if (target != base.Owner)
        {
            return target;
        }
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] SteedGuardPassive: redirect {amount} dmg from {target?.LogName}");
        var player = base.Owner.Player;
        if (player?.PlayerCombatState == null)
        {
            return target;
        }
        var glastrier = player.PlayerCombatState.GetPet<Glastrier>();
        if (glastrier != null && glastrier.IsAlive)
        {
            return glastrier;
        }
        var spectrier = player.PlayerCombatState.GetPet<Spectrier>();
        if (spectrier != null && spectrier.IsAlive)
        {
            return spectrier;
        }
        return target;
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
