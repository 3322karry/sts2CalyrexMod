using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using CalyrexMod.CardPools;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class CalyrexVisualPatches
{
    private const string EnergyIconTres = "res://CalyrexMod/icons/energy_calyrex.tres";

    // 卡牌能量图标：蕾冠王卡池用皇冠图标
    [HarmonyPatch(typeof(CardPoolModel), nameof(CardPoolModel.EnergyIconPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void EnergyIconPathPostfix(CardPoolModel __instance, ref string __result)
    {
        try
        {
            if (__instance is CalyrexCardPool)
            {
                __result = EnergyIconTres;
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] CalyrexVisualPatches: {ex}");
        }
    }

    // 0.113 起 EnergyIconPath 改为 EnergyIconHelper.GetPath(EnergyColorName)：
    // 卡面/卡牌库等路径会走 helper，直接改池属性可能被 JIT 内联绕过，
    // 因此同时 patch helper 的字符串重载，保证 "calyrex" 前缀始终指向 mod 的独立 .tres。
    [HarmonyPatch(typeof(EnergyIconHelper), nameof(EnergyIconHelper.GetPath), new[] { typeof(string) })]
    [HarmonyPostfix]
    private static void EnergyIconGetPathPostfix(string prefix, ref string __result)
    {
        try
        {
            if (string.Equals(prefix, "calyrex", StringComparison.OrdinalIgnoreCase))
            {
                __result = EnergyIconTres;
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] EnergyIconGetPath: {ex}");
        }
    }
}
