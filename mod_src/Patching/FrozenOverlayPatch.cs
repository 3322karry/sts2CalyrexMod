using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using CalyrexMod.Powers;

namespace CalyrexMod.Patching;

// 冰冻血条覆盖层：敌人血条显示淡蓝色"冰冻致死线"（层数×10 对应的血量区域，仿灾厄 doom 前景层）
[HarmonyPatch]
public static class FrozenOverlayPatch
{
    private static readonly Dictionary<Creature, ColorRect> _overlays = new Dictionary<Creature, ColorRect>();

    private const int FrozenMultiplier = 10;

    [HarmonyPatch(typeof(NHealthBar), "RefreshForeground")]
    [HarmonyPostfix]
    private static void RefreshForegroundPostfix(NHealthBar __instance)
    {
        try
        {
            var trav = HarmonyLib.Traverse.Create(__instance);
            var creature = trav.Field("_creature").GetValue<Creature>();
            if (creature == null)
            {
                return;
            }
            int frozen = creature.Powers.FirstOrDefault((MegaCrit.Sts2.Core.Models.PowerModel p) => p is FrozenPower)?.Amount ?? 0;
            var bar = trav.Field("_hpForeground").GetValue<Control>();
            var overlay = GetOrCreateOverlay(__instance, creature, bar);
            if (frozen <= 0 || bar == null)
            {
                overlay.Visible = false;
                return;
            }
            // 计算冰冻致死线宽度（同 doom：GetFgWidth(amount) 从右往左）
            float maxFgWidth = trav.Property("MaxFgWidth").GetValue<float>();
            float fgWidth = 0f;
            try
            {
                fgWidth = trav.Method("GetFgWidth", frozen * FrozenMultiplier).GetValue<float>();
            }
            catch (System.Exception)
            {
                fgWidth = maxFgWidth * (frozen * FrozenMultiplier) / 100f;
            }
            // 淡蓝色覆盖：从血条右端向左覆盖致死区域
            overlay.OffsetLeft = Mathf.Max(0f, fgWidth - maxFgWidth) + bar.OffsetLeft;
            overlay.OffsetRight = bar.OffsetRight;
            overlay.OffsetTop = bar.OffsetTop;
            overlay.OffsetBottom = bar.OffsetBottom;
            overlay.Visible = true;
        }
        catch (System.Exception)
        {
        }
    }

    private static ColorRect GetOrCreateOverlay(NHealthBar bar, Creature creature, Control? anchor)
    {
        if (_overlays.TryGetValue(creature, out var existing) && GodotObject.IsInstanceValid(existing))
        {
            return existing;
        }
        var rect = new ColorRect
        {
            Color = new Color(0.53f, 0.81f, 0.98f, 0.45f),  // 淡蓝色半透明
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Visible = false
        };
        if (anchor != null)
        {
            anchor.AddChild(rect);
        }
        else
        {
            bar.AddChild(rect);
        }
        _overlays[creature] = rect;
        return rect;
    }
}
