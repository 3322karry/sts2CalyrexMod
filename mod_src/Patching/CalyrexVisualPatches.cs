using HarmonyLib;
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
        catch (System.Exception ex)
        {
            Log.Info($"[CalyrexMod] CalyrexVisualPatches: {ex}");
        }
    }
}
