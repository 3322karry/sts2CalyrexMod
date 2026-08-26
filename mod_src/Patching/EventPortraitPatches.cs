using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Events;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class EventPortraitPatches
{
    [HarmonyPatch(typeof(EventModel), "get_InitialPortraitPath")]
    [HarmonyPostfix]
    private static void InitialPortraitPathPostfix(EventModel __instance, ref string __result)
    {
        try
        {
            if (__instance is CelebiExpress)
            {
                __result = "res://CalyrexMod/icons/events/celebi_express.tres";
            }
            else if (__instance is PokemonDaycare)
            {
                __result = "res://CalyrexMod/icons/events/pokemon_daycare.tres";
            }
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] EventPortrait patch failed: {ex}");
        }
    }
}
