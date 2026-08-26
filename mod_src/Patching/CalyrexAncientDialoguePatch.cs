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
            var dialogues = new AncientDialogue[]
            {
                new AncientDialogue("", ""),
                new AncientDialogue(""),
                new AncientDialogue("", "", "")
            };
            __result.CharacterDialogues[charKey] = dialogues;

            // 手动填充本地化文本（DialogueSet 已 Populate 过，需补我们的）
            string entry = __instance.Id.Entry;
            for (int i = 0; i < dialogues.Length; i++)
            {
                for (int j = 0; j < dialogues[i].Lines.Count; j++)
                {
                    string baseKey = $"{entry}.talk.{charKey}.{i}-{j}";
                    var line = dialogues[i].Lines[j];
                    if (LocString.Exists("ancients", baseKey + ".ancient"))
                    {
                        line.LineText = new LocString("ancients", baseKey + ".ancient");
                        line.Speaker = AncientDialogueSpeaker.Ancient;
                    }
                    else
                    {
                        line.LineText = new LocString("ancients", baseKey + ".char");
                        line.Speaker = AncientDialogueSpeaker.Character;
                    }
                    if (j < dialogues[i].Lines.Count - 1)
                    {
                        line.NextButtonText = new LocString("ancients", baseKey + ".next");
                    }
                }
            }
            Log.Info($"[CalyrexMod] Injected Calyrex dialogues into {entry}");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] CalyrexAncientDialoguePatch failed: {ex}");
        }
    }
}
