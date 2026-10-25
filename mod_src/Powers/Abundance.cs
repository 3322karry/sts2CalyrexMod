using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace CalyrexMod.Powers;

public sealed class Abundance : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 战斗结束回血：回复「丰饶层数 × 配置百分比」的生命（默认 50%，向下取整），可在 CalyrexMod.conf 关闭。
    // 注意：AfterCombatVictory 前玩家 powers 已被清理，所以用 AfterCombatEnd；
    // playAnim:false 避免在结算阶段创建 VFX 导致卡住。
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Owner == null || base.Owner.CombatState == null || base.Amount <= 0)
        {
            return;
        }

        if (!ModConfig.Data.AbundanceCombatHeal)
        {
            Log.Info($"[CalyrexMod] Abundance: combat heal disabled by config (stacks={base.Amount})");
            return;
        }

        int percent = ModConfig.Data.AbundanceCombatHealPercent;
        decimal heal = Math.Floor(base.Amount * percent / 100m);
        if (heal <= 0m)
        {
            Log.Info($"[CalyrexMod] Abundance: heal rounds down to 0 (stacks={base.Amount}, percent={percent}%)");
            return;
        }

        // 回复我方所有玩家（单机即自己）
        foreach (Player player in base.Owner.CombatState.Players)
        {
            if (player.Creature != null && !player.Creature.IsDead)
            {
                await CreatureCmd.Heal(player.Creature, heal, playAnim: false);
                Log.Info($"[CalyrexMod] Abundance: combat end heal {heal} (stacks={base.Amount}, percent={percent}%, player={player.NetId})");
            }
        }
    }
}
