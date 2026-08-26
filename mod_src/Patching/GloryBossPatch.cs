using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Acts;
using CalyrexMod.Events;

namespace CalyrexMod.Patching;

// 荣耀幕：无极汰那 Boss 替换原 AeonglassBoss（Boss 池）
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
            int idx = list.FindIndex((EncounterModel e) => e is AeonglassBoss);
            if (idx >= 0)
            {
                list[idx] = ModelDb.Encounter<EternatusBoss>();
                __result = list;
                Log.Info("[CalyrexMod] Glory: AeonglassBoss -> EternatusBoss");
            }
        }
        catch (System.Exception ex)
        {
            Log.Error($"[CalyrexMod] GloryBossPatch failed: {ex}");
        }
    }
}
