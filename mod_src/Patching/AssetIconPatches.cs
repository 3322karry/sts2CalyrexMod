using System;
using System.IO;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Patching;

// 卡面/遗物图标：patch 路径 getter 指向 mod 自己的 .tres（与药水同方案）
[HarmonyPatch]
public static class AssetIconPatches
{
    private const string CardPortraitRoot = "res://CalyrexMod/icons/card_portraits/{0}.tres";
    private const string RelicIconRoot = "res://CalyrexMod/icons/relics/{0}.tres";

    // 卡面：CardModel.get_PortraitPath -> res://CalyrexMod/icons/card_portraits/{slug}.tres
    [HarmonyPatch(typeof(CardModel), "get_PortraitPath")]
    [HarmonyPostfix]
    private static void PortraitPathPostfix(CardModel __instance, ref string __result)
    {
        try
        {
            string slug = __instance.Id.Entry.ToLowerInvariant();
            string path = string.Format(CardPortraitRoot, slug);
            if (ResourceLoader.Exists(path))
            {
                __result = path;
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] PortraitPath: {ex.Message}");
        }
    }

    // 遗物图标：RelicModel.get_PackedIconPath -> res://CalyrexMod/icons/relics/{entry}.tres
    [HarmonyPatch(typeof(RelicModel), "get_PackedIconPath")]
    [HarmonyPostfix]
    private static void RelicPackedIconPathPostfix(RelicModel __instance, ref string __result)
    {
        try
        {
            string name = __instance.Id.Entry.ToLowerInvariant();
            string path = string.Format(RelicIconRoot, name);
            if (ResourceLoader.Exists(path))
            {
                __result = path;
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] RelicPackedIcon: {ex.Message}");
        }
    }

    // 遗物大图标（详情页）：RelicModel.get_BigIconPath
    [HarmonyPatch(typeof(RelicModel), "get_BigIconPath")]
    [HarmonyPostfix]
    private static void RelicBigIconPathPostfix(RelicModel __instance, ref string __result)
    {
        try
        {
            string name = __instance.Id.Entry.ToLowerInvariant();
            string path = string.Format(RelicIconRoot, name);
            if (ResourceLoader.Exists(path))
            {
                __result = path;
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] RelicBigIcon: {ex.Message}");
        }
    }

    // 遗物轮廓图标（图鉴/商店拖影）：RelicModel.get_PackedIconOutlinePath
    [HarmonyPatch(typeof(RelicModel), "get_PackedIconOutlinePath")]
    [HarmonyPostfix]
    private static void RelicOutlinePathPostfix(RelicModel __instance, ref string __result)
    {
        try
        {
            string name = __instance.Id.Entry.ToLowerInvariant();
            string path = string.Format(RelicIconRoot, name);
            if (ResourceLoader.Exists(path))
            {
                __result = path;
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] RelicOutline: {ex.Message}");
        }
    }
}
