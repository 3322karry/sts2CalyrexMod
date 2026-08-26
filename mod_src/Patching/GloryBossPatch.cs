using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Acts;
using CalyrexMod.Events;

namespace CalyrexMod.Patching;

// 荣耀幕：无极汰那加入 Boss 池（不替换原 Boss，4 个 Boss 随机出现）
[HarmonyPatch]
public static class GloryBossPatch
{
    [HarmonyPatch(typeof(Glory), "GenerateAllEncounters")]
    [HarmonyPostfix]
    private static void GenerateAllEncountersPostfix(ref IEnumerable<EncounterModel> __result)
    {
        try
        {
            var list = __result.ToList();
            if (list.Any((EncounterModel e) => e is EternatusBoss))
            {
                return;
            }
            list.Add(ModelDb.Encounter<EternatusBoss>());
            __result = list;
            Log.Info("[CalyrexMod] Glory: added EternatusBoss to boss pool (random)");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] GloryBossPatch failed: {ex}");
        }
    }
}
