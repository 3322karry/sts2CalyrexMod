using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using CalyrexMod.Monsters;

namespace CalyrexMod.Patching;

// 让宠物马成为可攻击/可指定目标（花粉团可奶马、爆音波 AOE 波及马）
// 聚光灯：场上有聚光灯目标时，AOE 只命中该目标
[HarmonyPatch]
public static class CombatTargetPatches
{
    public static Creature? GetSpotlightTarget(CombatState combatState)
    {
        try
        {
            return combatState.Enemies.FirstOrDefault((Creature e) => e.IsAlive && e.HasPower<CalyrexMod.Powers.SpotlightPower>());
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    [HarmonyPatch(typeof(CombatState), "GetOpponentsOf")]
    [HarmonyPostfix]
    private static void GetOpponentsOfPostfix(CombatState __instance, Creature creature, ref System.Collections.Generic.IReadOnlyList<Creature> __result)
    {
        try
        {
            if (creature.IsPlayer)
            {
                var spot = GetSpotlightTarget(__instance);
                if (spot != null && __result.Any((Creature e) => e == spot))
                {
                    __result = new Creature[] { spot };
                }
            }
        }
        catch (System.Exception)
        {
        }
    }

    [HarmonyPatch(typeof(CombatState), "get_HittableEnemies")]
    [HarmonyPostfix]
    private static void HittableEnemiesPostfix(CombatState __instance, ref System.Collections.Generic.IReadOnlyList<Creature> __result)
    {
        try
        {
            var steeds = __instance.Players
                .SelectMany((Player p) => p.PlayerCombatState?.Pets ?? Enumerable.Empty<Creature>())
                .Where((Creature c) => c is Glastrier || c is Spectrier)
                .Where((Creature c) => c.IsAlive && c.IsHittable)
                .ToList();
            if (steeds.Count > 0)
            {
                __result = __result.Concat(steeds).ToList();
                MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] HittableEnemies: added {steeds.Count} steeds, total {__result.Count}");
            }
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] HittableEnemies patch: {ex.Message}");
        }
    }
}
