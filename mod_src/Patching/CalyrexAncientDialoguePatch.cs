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

            string entry = __instance.Id.Entry;
            // 按用户提供的对话：单组（行数按先古：涅奥 3 行、其他 1 行）
            // VisitIndex=0 + IsRepeating=true：任意访问次数都显示同一段对话
            // 涅奥有蕾冠王回复（char 行），其他先古只有一句
            int lineCount = (entry == "NEOW") ? 3 : 1;
            string[] sfx = new string[lineCount];
            for (int k = 0; k < lineCount; k++)
            {
                sfx[k] = "";
            }
            var dialogue = new AncientDialogue(sfx) { VisitIndex = 0, IsRepeating = true };
            __result.CharacterDialogues[charKey] = new[] { dialogue };

            // 直接构造 LocString（不查 Exists——注入时机可能早于 mod 表合并，渲染时再解析）
            // 行结构：偶数行 ancient（先古说话）、奇数行 char（蕾冠王说话）
            for (int j = 0; j < dialogue.Lines.Count; j++)
            {
                string baseKey = $"{entry}.talk.{charKey}.0-{j}";
                var line = dialogue.Lines[j];
                bool isAncientLine = (j % 2 == 0);
                line.LineText = new LocString("ancients", baseKey + (isAncientLine ? ".ancient" : ".char"));
                line.Speaker = isAncientLine ? AncientDialogueSpeaker.Ancient : AncientDialogueSpeaker.Character;
                if (j < dialogue.Lines.Count - 1)
                {
                    line.NextButtonText = new LocString("ancients", baseKey + ".next");
                }
            }
            if (entry == "NEOW")
            {
                MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] NEOW dialog raw0 = '{dialogue.Lines[0].LineText?.GetRawText()}'");
            }
            Log.Info($"[CalyrexMod] Injected Calyrex dialogues into {entry}");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] CalyrexAncientDialoguePatch failed: {ex}");
        }
    }
}
