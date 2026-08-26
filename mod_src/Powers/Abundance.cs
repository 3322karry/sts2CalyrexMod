using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace CalyrexMod.Powers;

public sealed class Abundance : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 战斗结束回血（回复我方所有玩家）。注意：AfterCombatVictory 前玩家 powers 已被清理，所以用 AfterCombatEnd；
    // playAnim:false 避免在结算阶段创建 VFX 导致卡住。
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Owner == null || base.Owner.CombatState == null || base.Amount <= 0)
        {
            return;
        }

        // 回复我方所有玩家（单机即自己）
        foreach (Player player in base.Owner.CombatState.Players)
        {
            if (player.Creature != null && !player.Creature.IsDead)
            {
                await CreatureCmd.Heal(player.Creature, base.Amount, playAnim: false);
            }
        }
    }
}
