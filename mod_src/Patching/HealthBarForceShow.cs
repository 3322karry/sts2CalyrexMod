using System.Threading.Tasks;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class HealthBarForceShow
{
    [HarmonyPatch(typeof(NCreatureStateDisplay), "RefreshValues")]
    [HarmonyPostfix]
    private static void OnRefreshValues(NCreatureStateDisplay __instance)
    {
        try
        {
            var creature = HarmonyLib.Traverse.Create(__instance).Field("_creature").GetValue() as MegaCrit.Sts2.Core.Entities.Creatures.Creature;
            if (creature?.PetOwner == null || !creature.IsAlive)
            {
                return;
            }
            __instance.Visible = true;
            var mod = __instance.Modulate;
            mod.A = 1f;
            __instance.Modulate = mod;
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] hbfs refresh: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(NCreatureStateDisplay), "AnimateIn", new[] { typeof(HealthBarAnimMode) })]
    [HarmonyPostfix]
    private static void OnAnimateIn(NCreatureStateDisplay __instance)
    {
        try
        {
            var creature = HarmonyLib.Traverse.Create(__instance).Field("_creature").GetValue() as MegaCrit.Sts2.Core.Entities.Creatures.Creature;
            if (creature?.PetOwner == null)
            {
                return;
            }
            __instance.Visible = true;
            var mod = __instance.Modulate;
            mod.A = 1f;
            __instance.Modulate = mod;
            var sd = __instance;
            _ = CheckLater(sd, creature);
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] hbfs: {ex.Message}");
        }
    }

    private static async Task CheckLater(NCreatureStateDisplay sd, MegaCrit.Sts2.Core.Entities.Creatures.Creature creature)
    {
        try
        {
            await sd.ToSignal(sd.GetTree(), SceneTree.SignalName.ProcessFrame);
            await sd.ToSignal(sd.GetTree(), SceneTree.SignalName.ProcessFrame);
            Log.Info($"[CalyrexMod] hbfs: {creature.Monster?.Id.Entry} final visible={sd.Visible} modulateA={sd.Modulate.A} pos={sd.GlobalPosition} size={sd.Size}");
            if (!sd.Visible || sd.Modulate.A < 0.5f)
            {
                sd.Visible = true;
                var m = sd.Modulate;
                m.A = 1f;
                sd.Modulate = m;
                Log.Info("[CalyrexMod] hbfs: re-forced visible+alpha");
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] hbfs later: {ex.Message}");
        }
    }
}
