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
                __result = new AncientDialogue[]
                {
                    new AncientDialogue("event:/sfx/ui/enchant_simple")
                    {
                        IsRepeating = true
                    }
                };
            }
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] ArchitectDialogueFix failed: {ex}");
        }
    }
}
