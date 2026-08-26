using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Potions;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class PotionIconPatches
{
    private static string? GetFallbackIcon(PotionModel potion)
    {
        return potion switch
        {
            FigyBerry => "res://CalyrexMod/icons/potions/figy_berry.tres",
            GrayCarrot => "res://CalyrexMod/icons/potions/gray_carrot.tres",
            DefenseBoost => "res://CalyrexMod/icons/potions/defense_boost.tres",
            GalarianSpice => "res://CalyrexMod/icons/potions/galarian_spice.tres",
            VictorsCurry => "res://CalyrexMod/icons/potions/victors_curry.tres",
            _ => null
        };
    }

    [HarmonyPatch(typeof(PotionModel), "get_PackedImagePath")]
    [HarmonyPostfix]
    private static void PackedImagePathPostfix(PotionModel __instance, ref string __result)
    {
        string? fallback = GetFallbackIcon(__instance);
        if (fallback != null)
        {
            Log.Info($"[CalyrexMod] PotionIcon PACKED: {__instance.GetType().Name} -> {fallback}");
            __result = fallback;
        }
    }

    [HarmonyPatch(typeof(PotionModel), "get_ImagePath")]
    [HarmonyPostfix]
    private static void ImagePathPostfix(PotionModel __instance, ref string __result)
    {
        string? fallback = GetFallbackIcon(__instance);
        if (fallback != null)
        {
            Log.Info($"[CalyrexMod] PotionIcon PATH: {__instance.GetType().Name} -> {fallback}");
            __result = fallback;
        }
    }

    [HarmonyPatch(typeof(PotionModel), "get_Image")]
    [HarmonyPostfix]
    private static void ImagePostfix(PotionModel __instance, ref Texture2D __result)
    {
        string? fallback = GetFallbackIcon(__instance);
        if (fallback != null)
        {
            Log.Info($"[CalyrexMod] PotionIcon IMG: {__instance.GetType().Name}");
            var tex = ResourceLoader.Load<Texture2D>(fallback, null, ResourceLoader.CacheMode.Reuse);
            if (tex != null)
            {
                __result = tex;
            }
        }
    }
}
