using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using CalyrexMod.Relics;

namespace CalyrexMod.Patching;

// 欧罗巴斯：为蕾冠王追加专属的"欧罗巴斯之触"（升级初始遗物+初始牌）
[HarmonyPatch]
public static class OrobasPatch
{
    [HarmonyPatch(typeof(Orobas), "get_AllPossibleOptions")]
    [HarmonyPostfix]
    private static void AllPossibleOptionsPostfix(Orobas __instance, ref IEnumerable<EventOption> __result)
    {
        try
        {
            var touch = (CalyrexOrobasTouch)ModelDb.Relic<CalyrexOrobasTouch>().ToMutable();
            if (__instance.Owner != null && !touch.SetupForPlayer(__instance.Owner))
            {
                return;
            }
            var option = EventOption.FromRelic(touch, __instance, () => Task.CompletedTask, "OROBAS.pages.INITIAL.options.CALYREX_TOUCH");
            __result = __result.Concat(new[] { option });
            Log.Info("[CalyrexMod] Orobas: added CalyrexOrobasTouch option");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] OrobasPatch failed: {ex}");
        }
    }
}
