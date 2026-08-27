using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Characters;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class ArchitectDialogueFix
{
    // 官方先古事件（如建筑师）只给 5 个角色配了对话；
    // 我们的角色没有对应 key 时 GetValidDialogues 返回空列表，
    // 导致 Rng.NextItem(空) 抛异常卡死。这里兜底返回通用对话。
    [HarmonyPatch(typeof(AncientDialogueSet), "GetValidDialogues")]
    [HarmonyPostfix]
    private static void GetValidDialoguesPostfix(AncientDialogueSet __instance, ModelId characterId, ref IEnumerable<AncientDialogue> __result)
    {
        try
        {
            if (characterId.Entry == "CALYREX_CHARACTER" && !__result.Any())
            {
                // 兜底：返回完整的蕾冠王对话（3 行），由 PopulateLines 填充本地化文本
                var dialogue = new AncientDialogue("", "", "");
                dialogue.PopulateLines("THE_ARCHITECT", "CALYREX_CHARACTER", 0);
                __result = new AncientDialogue[] { dialogue };
            }
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] ArchitectDialogueFix failed: {ex}");
        }
    }
}
