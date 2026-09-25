using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.Formatters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using SmartFormat.Core.Extensions;
using MegaCrit.Sts2.Core.Logging;

namespace CalyrexMod.Patching;

// 描述中的费用图标：官方 [img] 加载 png 需要 .import（mod 无法提供），
// 改为输出我们自己的 .tres 图片路径（ImageTexture，可被 ResourceLoader 加载）。
// 仅当图标前缀为 calyrex（蕾冠王卡池 / 蕾冠王角色）时替换，其余角色卡牌走官方原版图标。
[HarmonyPatch]
public static class EnergyIconFormatterPatch
{
    private const string CalyrexIcon = "res://CalyrexMod/icons/energy_icon_24.tres";

    [HarmonyPatch(typeof(EnergyIconsFormatter), "TryEvaluateFormat")]
    [HarmonyPrefix]
    private static bool TryEvaluateFormatPrefix(IFormattingInfo formattingInfo, ref bool __result)
    {
        try
        {
            // 解析图标前缀（与官方逻辑一致）：卡牌自带 ColorPrefix → energyPrefix 字符串变量 → 本地角色卡池前缀
            string? prefix = null;
            object currentValue = formattingInfo.CurrentValue;
            if (currentValue is string strVal && !string.IsNullOrEmpty(strVal))
            {
                // {energyPrefix:energyIcons(N)}：字符串值本身就是前缀（卡牌库等无 run 场景也能用）
                prefix = strVal;
            }
            else if (currentValue is MegaCrit.Sts2.Core.Localization.DynamicVars.EnergyVar energyVar
                && !string.IsNullOrEmpty(energyVar.ColorPrefix))
            {
                prefix = energyVar.ColorPrefix;
            }
            if (string.IsNullOrEmpty(prefix) || prefix == "colorless")
            {
                prefix = RunManager.Instance.GetLocalCharacterEnergyIconPrefix();
            }
            // 非蕾冠王：交回官方逻辑（原版 *_energy_icon.png）
            if (prefix != "calyrex")
            {
                return true;
            }

            int count;
            if (currentValue is MegaCrit.Sts2.Core.Localization.DynamicVars.EnergyVar ev)
            {
                count = (int)ev.PreviewValue;
            }
            else if (currentValue is MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar dynVar)
            {
                count = dynVar.IntValue;
            }
            else if (currentValue is decimal dec)
            {
                count = (int)dec;
            }
            else if (currentValue is int i)
            {
                count = i;
            }
            else if (currentValue is string s)
            {
                if (!int.TryParse(formattingInfo.FormatterOptions, out count))
                {
                    __result = false;
                    return false;
                }
                _ = s;
            }
            else
            {
                __result = false;
                return false;
            }

            string img = $"[img]{CalyrexIcon}[/img]";
            string output = count > 0 ? string.Concat(Enumerable.Repeat(img, count)) : "";
            formattingInfo.Write(output);
            __result = true;
            return false;
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] EnergyIconFormatterPatch failed: {ex}");
            __result = false;
            return false;
        }
    }
}
