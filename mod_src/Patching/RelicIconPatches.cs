using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Relics;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class RelicIconPatches
{
    private static string? GetFallbackIcon(RelicModel relic)
    {
        return relic switch
        {
            BlackWhiteCarrot => "res://CalyrexMod/icons/relics/black_white_carrot.tres",
            SereneGrace => "res://CalyrexMod/icons/relics/serene_grace.tres",
            FocusSash => "res://CalyrexMod/icons/relics/focus-sash.tres",
            NeverMeltIce => "res://CalyrexMod/icons/relics/never-melt-ice.tres",
            SpellTag => "res://CalyrexMod/icons/relics/spell-tag.tres",
            Eviolite => "res://CalyrexMod/icons/relics/eviolite.tres",
            Multiscale => "res://CalyrexMod/icons/relics/multiscale.tres",
            ExpShare => "res://CalyrexMod/icons/relics/exp-share.tres",
            MiracleSeed => "res://CalyrexMod/icons/relics/miracle-seed.tres",
            LoadedDice => "res://CalyrexMod/icons/relics/loaded_dice.tres",
            Disguise => "res://CalyrexMod/icons/relics/disguise.tres",
            SnowCarrot => "res://CalyrexMod/icons/relics/snow_carrot.tres",
            _ => null
        };
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.PackedIconPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void PackedIconPathPostfix(RelicModel __instance, ref string __result)
    {
        string? fallback = GetFallbackIcon(__instance);
        if (fallback != null)
        {
            __result = fallback;
        }
    }

    [HarmonyPatch(typeof(RelicModel), "get_BigIconPath")]
    [HarmonyPostfix]
    private static void BigIconPathPostfix(RelicModel __instance, ref string __result)
    {
        string? fallback = GetFallbackIcon(__instance);
        if (fallback != null)
        {
            __result = fallback;
        }
    }
}
