using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.Formatters;
using SmartFormat.Core.Extensions;
using MegaCrit.Sts2.Core.Logging;

namespace CalyrexMod.Patching;

// 描述中的费用图标：官方 [img] 加载 png 需要 .import（mod 无法提供），
// 改为输出我们自己的 .tres 图片路径（ImageTexture，可被 ResourceLoader 加载）。
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
            int count;
            object currentValue = formattingInfo.CurrentValue;
            if (currentValue is MegaCrit.Sts2.Core.Localization.DynamicVars.EnergyVar energyVar)
            {
                count = (int)energyVar.PreviewValue;
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
