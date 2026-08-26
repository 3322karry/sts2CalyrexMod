using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using CalyrexMod.Powers;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class MountedVisualPatches
{
    public const string MountedGlastrierVisualPath = "res://CalyrexMod/scenes/calyrex_ice.tscn";
    public const string MountedSpectrierVisualPath = "res://CalyrexMod/scenes/calyrex_shadow.tscn";
    public const string NormalVisualPath = "res://CalyrexMod/scenes/calyrex.tscn";

    public static void ApplyVisual(Player player, string scenePath)
    {
        try
        {
            NCombatRoom? room = NCombatRoom.Instance;
            NCreature? node = room?.GetCreatureNode(player.Creature);
            if (node == null)
            {
                Log.Info($"[CalyrexMod] MountedVisual: creature node not found");
                return;
            }

            NCreatureVisuals newVisuals = PreloadManager.Cache.GetScene(scenePath).Instantiate<NCreatureVisuals>(PackedScene.GenEditState.Disabled);
            newVisuals.Name = "Visuals";

            var trav = Traverse.Create(node).Property("Visuals");
            NCreatureVisuals? oldVisuals = trav.GetValue<NCreatureVisuals>();
            if (oldVisuals != null && GodotObject.IsInstanceValid(oldVisuals))
            {
                node.RemoveChildSafely(oldVisuals);
                oldVisuals.QueueFreeSafely();
            }

            node.AddChildSafely(newVisuals);
            node.MoveChildSafely(newVisuals, 0);
            trav.SetValue(newVisuals);

            // 重新计算 Hitbox/血条位置（基于新视觉 Bounds）
            Traverse.Create(node).Method("UpdateBounds", newVisuals).GetValue();
            Log.Info($"[CalyrexMod] MountedVisual: replaced visuals -> {scenePath}");
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] MountedVisual: {ex}");
        }
    }

    public static bool IsMounted(Creature creature)
    {
        return creature.Powers.Any((PowerModel p) => p is MountedGlastrier || p is MountedSpectrier);
    }

    [HarmonyPatch(typeof(Creature), "CreateVisuals")]
    [HarmonyPrefix]
    private static bool CreateVisualsPrefix(Creature __instance, ref NCreatureVisuals? __result)
    {
        try
        {
            if (__instance.Player != null && IsMounted(__instance))
            {
                string path = __instance.Powers.Any((PowerModel p) => p is MountedGlastrier)
                    ? MountedGlastrierVisualPath
                    : MountedSpectrierVisualPath;
                __result = PreloadManager.Cache.GetScene(path).Instantiate<NCreatureVisuals>(Godot.PackedScene.GenEditState.Disabled);
                Log.Info($"[CalyrexMod] MountedVisual: player visuals -> {path}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] MountedVisual: {ex.Message}");
        }
        return true;
    }
}
