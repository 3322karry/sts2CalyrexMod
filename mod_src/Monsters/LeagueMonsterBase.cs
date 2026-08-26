using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Monsters;

// 联赛换人基类：死亡后触发下一个敌人上场。
// 换人目标用静态映射（按怪物类型），避免实例属性在战斗克隆中丢失/污染。
public abstract class LeagueMonsterBase : MonsterModel
{
    // 静态换人表：怪物类型 -> 下一个类型（null=不再换）；支持随机候选
    private static readonly Dictionary<Type, Type> _nextMap = new Dictionary<Type, Type>();
    private static readonly Dictionary<Type, Type[]> _nextRandomMap = new Dictionary<Type, Type[]>();

    public static void RegisterNext(Type from, Type to)
    {
        _nextMap[from] = to;
    }

    public static void RegisterNextRandom(Type from, params Type[] candidates)
    {
        _nextRandomMap[from] = candidates;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
        if (wasRemovalPrevented)
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
            var myType = creature.Monster?.GetType();
            if (myType == null)
            {
                return;
            }
            Type nextType;
            if (_nextRandomMap.TryGetValue(myType, out var candidates) && candidates.Length > 0)
            {
                nextType = candidates[System.Random.Shared.Next(candidates.Length)];
            }
            else if (!_nextMap.TryGetValue(myType, out nextType))
            {
                return;
            }
            var aliveList = string.Join(",", combatState.Enemies.Where((Creature e) => e.IsAlive).Select((Creature e) => e.Monster?.Id.Entry ?? "?"));
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] League swap: {creature.Monster?.Id.Entry} died, Next={nextType.Name}, slot={creature.SlotName}, alive=[{aliveList}]");
            var next = (MonsterModel)ModelDb.GetById<MonsterModel>(ModelDb.GetId(nextType)).ToMutable();
            var newCreature = await CreatureCmd.Add(next, combatState, CombatSide.Enemy, creature.SlotName);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] League spawned: {newCreature.Monster?.Id.Entry} slot={newCreature.SlotName}");
            // 新敌人上场先晕一回合（意图显示为晕，下回合再行动）
            newCreature.Monster?.OnSideSwitch();
            if (newCreature.IsAlive)
            {
                await CreatureCmd.Stun(newCreature, (_) => Task.CompletedTask);
                MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] League stunned newcomer: {newCreature.Monster?.Id.Entry}");
            }
            combatState.RemoveCreature(creature);
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] League swap failed: {ex}");
        }
    }
}
