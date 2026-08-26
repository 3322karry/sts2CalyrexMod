using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Events;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class EventPoolPatches
{
    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllSharedEvents), MethodType.Getter)]
    [HarmonyPostfix]
    private static void AllSharedEventsPostfix(ref IEnumerable<EventModel> __result)
    {
        var ours = new EventModel[]
        {
            ModelDb.Event<CelebiExpress>(),
            ModelDb.Event<PokemonDaycare>(),
            ModelDb.Event<PokemonLeague>()
        };
        __result = __result.Concat(ours);
    }
}
