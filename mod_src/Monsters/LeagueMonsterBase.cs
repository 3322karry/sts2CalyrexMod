using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Monsters;

// 联赛换人基类：死亡后触发下一个敌人上场
public abstract class LeagueMonsterBase : MonsterModel
{
    public System.Type? NextMonsterType { get; set; }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
        if (NextMonsterType == null || wasRemovalPrevented)
        {
            return;
        }
        try
        {
            var combatState = creature.CombatState;
            if (combatState == null || !combatState.IsLiveCombat())
            {
                return;
            }
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] League swap: {creature.Monster?.Id.Entry} -> {NextMonsterType.Name}");
            var next = (MonsterModel)ModelDb.GetById<MonsterModel>(ModelDb.GetId(NextMonsterType)).ToMutable();
            if (next is LeagueMonsterBase lb)
            {
                lb.NextMonsterType = NextMonsterType;
            }
            var newCreature = await CreatureCmd.Add(next, combatState, CombatSide.Enemy, creature.SlotName);
            // 让新敌人当回合立即行动（重置 SpawnedThisTurn）
            newCreature.Monster?.OnSideSwitch();
            combatState.RemoveCreature(creature);
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] League swap failed: {ex}");
        }
    }
}
