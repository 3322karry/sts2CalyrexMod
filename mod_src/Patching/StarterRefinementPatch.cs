using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using CalyrexMod.Cards;
using CalyrexMod.Relics;

namespace CalyrexMod.Patching;

// 让原版遗物识别蕾冠王初始内容：
// TouchOfOrobas 升级黑白萝卜→灵雪萝卜；ArchaicTooth 变化牵绊缰绳→勇气绳索
[HarmonyPatch]
public static class StarterRefinementPatch
{
    [HarmonyPatch(typeof(TouchOfOrobas), "get_RefinementUpgrades")]
    [HarmonyPostfix]
    private static void RefinementUpgradesPostfix(ref Dictionary<ModelId, RelicModel> __result)
    {
        try
        {
            __result[ModelDb.Relic<BlackWhiteCarrot>().Id] = ModelDb.Relic<SnowCarrot>();
            Log.Info("[CalyrexMod] TouchOfOrobas: BlackWhiteCarrot -> SnowCarrot registered");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] RefinementUpgradesPostfix failed: {ex}");
        }
    }

    [HarmonyPatch(typeof(ArchaicTooth), "get_TranscendenceUpgrades")]
    [HarmonyPostfix]
    private static void TranscendenceUpgradesPostfix(ref Dictionary<ModelId, CardModel> __result)
    {
        try
        {
            __result[ModelDb.Card<BondedReins>().Id] = ModelDb.Card<CourageRope>();
            Log.Info("[CalyrexMod] ArchaicTooth: BondedReins -> CourageRope registered");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] TranscendenceUpgradesPostfix failed: {ex}");
        }
    }
}
