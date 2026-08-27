using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using CalyrexMod.Monsters;

namespace CalyrexMod.Patching;

// 让宠物马成为可攻击/可指定目标（花粉团可奶马、爆音波 AOE 波及马）
[HarmonyPatch]
public static class CombatTargetPatches
{
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
            }
        }
        catch (System.Exception)
        {
        }
    }
}
