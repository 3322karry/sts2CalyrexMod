using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Patching;

// 为所有先古事件注入蕾冠王的专属对话。
// 对话文本 key：{ANCIENT}.talk.CALYREX_CHARACTER.{i}-{j}.ancient/.char/.next
[HarmonyPatch]
public static class CalyrexAncientDialoguePatch
{
    [HarmonyPatch(typeof(AncientEventModel), "get_DialogueSet")]
    [HarmonyPostfix]
    private static void DialogueSetPostfix(AncientEventModel __instance, ref AncientDialogueSet __result)
    {
        try
        {
            string charKey = "CALYREX_CHARACTER";
            if (__result.CharacterDialogues.ContainsKey(charKey))
            {
                return;
            }

            // 三组对话：初见(0)、重复(1)、特殊(2)。每组行数不同，空串 sfx 走 fallback。
            // 必须设置 VisitIndex——GetValidDialogues 按 d.VisitIndex == charVisits 筛选，
            // 不设置（null）会导致对话被过滤而显示官方通用文本
            var dialogues = new AncientDialogue[]
            {
                new AncientDialogue("", "") { VisitIndex = 0 },
                new AncientDialogue("") { VisitIndex = 1, IsRepeating = true },
                new AncientDialogue("", "", "") { VisitIndex = 2 }
            };
            __result.CharacterDialogues[charKey] = dialogues;

            // 手动填充本地化文本（DialogueSet 已 Populate 过，需补我们的）
            // 直接构造 LocString（不查 Exists——注入时机可能早于 mod 表合并，渲染时再解析）
            // 行结构：偶数行 ancient（先古说话）、奇数行 char（蕾冠王说话）；第 1 组（重复）key 带 r 后缀
            string entry = __instance.Id.Entry;
            for (int i = 0; i < dialogues.Length; i++)
            {
                string groupSuffix = (i == 1) ? "r" : "";
                for (int j = 0; j < dialogues[i].Lines.Count; j++)
                {
                    string baseKey = $"{entry}.talk.{charKey}.{i}-{j}{groupSuffix}";
                    var line = dialogues[i].Lines[j];
                    bool isAncientLine = (j % 2 == 0);
                    line.LineText = new LocString("ancients", baseKey + (isAncientLine ? ".ancient" : ".char"));
                    line.Speaker = isAncientLine ? AncientDialogueSpeaker.Ancient : AncientDialogueSpeaker.Character;
                    if (j < dialogues[i].Lines.Count - 1)
                    {
                        line.NextButtonText = new LocString("ancients", baseKey + ".next");
                    }
                }
            }
            if (entry == "NEOW" && dialogues.Length > 0 && dialogues[0].Lines.Count > 0)
            {
                MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] NEOW dialog raw0 = '{dialogues[0].Lines[0].LineText?.GetRawText()}'");
            }
            Log.Info($"[CalyrexMod] Injected Calyrex dialogues into {entry}");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] CalyrexAncientDialoguePatch failed: {ex}");
        }
    }
}
