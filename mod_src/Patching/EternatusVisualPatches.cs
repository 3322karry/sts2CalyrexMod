using System;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Assets;
using CalyrexMod.Monsters;
using CalyrexMod.Events;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Helpers;

namespace CalyrexMod.Patching;

// 无极汰那视觉：Boss 战背景 + 阶段2 无极巨化精灵（替换 Visuals + 放大 3 倍）
[HarmonyPatch]
public static class EternatusVisualPatches
{
    public const string EternatusBgPath = "res://CalyrexMod/icons/events/eternatus_bg.tres";
    public const string EternamaxVisualPath = "res://CalyrexMod/monsters/eternatus_eternamax.tscn";

    // 进入 Boss 战时覆盖背景
    [HarmonyPatch(typeof(NCombatBackground), "Create")]
    [HarmonyPostfix]
    private static void BackgroundPostfix(NCombatBackground __result)
    {
        try
        {
            var state = CombatManager.Instance.DebugOnlyGetState();
            if (state?.Encounter == null || !(state.Encounter is EternatusBoss) || __result == null)
            {
                return;
            }
            var tex = ResourceLoader.Load<Texture2D>(EternatusBgPath, null, ResourceLoader.CacheMode.Reuse);
            if (tex == null)
            {
                Log.Info($"[CalyrexMod] Eternatus bg load failed");
                return;
            }
            var rect = new TextureRect
            {
                Texture = tex,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                ZIndex = 50
            };
            rect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            __result.AddChild(rect);
            Log.Info("[CalyrexMod] Eternatus bg overlay applied");
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] Eternatus bg: {ex.Message}");
        }
    }

    // 阶段2 无极巨化：替换 Visuals 并放大 3 倍（由 Eternatus.RespawnMove 调用）
    public static void ApplyEternamaxVisual(Creature creature)
    {
        try
        {
            NCombatRoom? room = NCombatRoom.Instance;
            NCreature? node = room?.GetCreatureNode(creature);
            if (node == null)
            {
                Log.Info($"[CalyrexMod] Eternamax: creature node not found");
                return;
            }
            NCreatureVisuals newVisuals = PreloadManager.Cache.GetScene(EternamaxVisualPath).Instantiate<NCreatureVisuals>(PackedScene.GenEditState.Disabled);
            newVisuals.Name = "Visuals";

            var trav = HarmonyLib.Traverse.Create(node).Property("Visuals");
            NCreatureVisuals? oldVisuals = trav.GetValue<NCreatureVisuals>();
            if (oldVisuals != null && GodotObject.IsInstanceValid(oldVisuals))
            {
                node.RemoveChildSafely(oldVisuals);
                oldVisuals.QueueFreeSafely();
            }
            node.AddChildSafely(newVisuals);
            node.MoveChildSafely(newVisuals, 0);
            trav.SetValue(newVisuals);
            HarmonyLib.Traverse.Create(node).Method("UpdateBounds", newVisuals).GetValue();
            node.SetDefaultScaleTo(3f, 0.5f);
            Log.Info("[CalyrexMod] Eternamax visuals applied + scale 3x");
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] Eternamax: {ex}");
        }
    }
}

// 地图 Boss 图标：用官方 placeholder（Aeonglass 图标）+ hsv 变色（无极汰那紫色调）
[HarmonyPatch]
public static class EternatusBossMapPatch
{
    public const string PlaceholderIconPath = "res://images/map/placeholder/aeonglass_boss_icon";

    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Models.EncounterModel), "get_BossNodePath")]
    [HarmonyPostfix]
    private static void BossNodePathPostfix(EncounterModel __instance, ref string __result)
    {
        try
        {
            if (__instance is CalyrexMod.Events.EternatusBoss)
            {
                __result = PlaceholderIconPath;
            }
        }
        catch (System.Exception)
        {
        }
    }

    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.Screens.Map.NBossMapPoint), "_Ready")]
    [HarmonyPostfix]
    private static void BossMapPointReadyPostfix(MegaCrit.Sts2.Core.Nodes.Screens.Map.NBossMapPoint __instance)
    {
        try
        {
            var trav = HarmonyLib.Traverse.Create(__instance);
            var runState = trav.Property("_runState").GetValue<MegaCrit.Sts2.Core.Runs.IRunState>();
            if (runState?.Act?.BossEncounter is CalyrexMod.Events.EternatusBoss)
            {
                var placeholder = __instance.GetNodeOrNull<Godot.TextureRect>("%PlaceholderImage");
                if (placeholder != null)
                {
                    var shader = Godot.ResourceLoader.Load<Godot.Shader>("res://shaders/hsv.gdshader", null, Godot.ResourceLoader.CacheMode.Reuse);
                    if (shader != null)
                    {
                        var mat = new Godot.ShaderMaterial { Shader = shader };
                        mat.SetShaderParameter("h", 0.78f);
                        mat.SetShaderParameter("s", 1.0f);
                        mat.SetShaderParameter("v", 1.0f);
                        placeholder.Material = mat;
                        MegaCrit.Sts2.Core.Logging.Log.Info("[CalyrexMod] Eternatus boss map icon: hue-shifted");
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] BossMapPointReady: {ex.Message}");
        }
    }
}
