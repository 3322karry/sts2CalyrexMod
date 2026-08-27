using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using CalyrexMod.Cards;

namespace CalyrexMod.Patching;

// 无色奖励不应刷出体系卡（归零/星碎/雪矛），它们由专属卡牌生成
[HarmonyPatch]
public static class ColorlessPoolFix
{
    [HarmonyPatch(typeof(CardPoolModel), "GetUnlockedCards")]
    [HarmonyPostfix]
    private static void GetUnlockedCardsPostfix(CardPoolModel __instance, ref System.Collections.Generic.IEnumerable<CardModel> __result)
    {
        try
        {
            if (__instance is ColorlessCardPool)
            {
                __result = __result.Where((CardModel c) => c is not Zero && c is not AstralBarrage && c is not GlacialLance);
            }
        }
        catch (System.Exception)
        {
        }
    }
}
