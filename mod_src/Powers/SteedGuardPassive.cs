using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Monsters;

namespace CalyrexMod.Powers;

// 马匹守护（玩家侧）：受到的攻击伤害由马承受。
// 挂载在蕾冠王身上（可靠参与 hook）。
public sealed class SteedGuardPassive : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldPlayVfx => false;

    private Creature? _activeSteed;
    private decimal _pendingDamage;

    // 玩家受到攻击伤害时：伤害减 0（由马承受），记录原伤害
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return amount;
        }
        if (!props.IsPoweredAttack())
        {
            return amount;
        }
        var player = base.Owner.Player;
        if (player?.PlayerCombatState == null)
        {
            return amount;
        }
        var glastrier = player.PlayerCombatState.GetPet<Glastrier>();
        if (glastrier != null && glastrier.IsAlive)
        {
            _activeSteed = glastrier;
            _pendingDamage = amount;
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] SteedGuardPassive: redirect {amount} dmg to Glastrier");
            return 0m;
        }
        var spectrier = player.PlayerCombatState.GetPet<Spectrier>();
        if (spectrier != null && spectrier.IsAlive)
        {
            _activeSteed = spectrier;
            _pendingDamage = amount;
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] SteedGuardPassive: redirect {amount} dmg to Spectrier");
            return 0m;
        }
        _activeSteed = null;
        _pendingDamage = 0m;
        return amount;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || _activeSteed == null)
        {
            return;
        }
        decimal dmg = _pendingDamage;
        _pendingDamage = 0m;
        var steed = _activeSteed;
        _activeSteed = null;
        if (steed.IsAlive && dmg > 0m)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), steed, dmg, ValueProp.Unblockable | ValueProp.Unpowered, dealer, cardSource);
        }
    }
}
