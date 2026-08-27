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
