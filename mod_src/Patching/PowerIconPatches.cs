using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class PowerIconPatches
{
    private static string? GetFallbackIcon(PowerModel power)
    {
        return power switch
        {
            EternalWhinny => "res://CalyrexMod/icons/markers/eternal_whinny.tres",
            Abundance => "res://CalyrexMod/icons/markers/abundance.tres",
            QuickSight => "res://CalyrexMod/icons/markers/quick_sight.tres",
            HeavyLance => "res://CalyrexMod/icons/markers/heavy_lance.tres",
            MountedGlastrier => "res://CalyrexMod/icons/markers/mounted_glastrier.tres",
            MountedSpectrier => "res://CalyrexMod/icons/markers/mounted_spectrier.tres",
            SteedGuard => "res://CalyrexMod/icons/markers/steed_guard.tres",
            DynamaxFormPower => "res://CalyrexMod/icons/markers/dynamax_form.tres",
            TemporaryThornsPower => "res://CalyrexMod/icons/markers/temp_thorns.tres",
            HelpingHandPower => "res://CalyrexMod/icons/markers/helping_hand.tres",
            GrassyTerrainPower => "res://CalyrexMod/icons/markers/grassy_terrain.tres",
            TrucePower => "res://CalyrexMod/icons/markers/truce.tres",
            FutureSightPower => "res://CalyrexMod/icons/markers/future_sight.tres",
            IceWallPower => "res://CalyrexMod/icons/markers/ice_wall.tres",
            FrozenPower => "res://CalyrexMod/icons/markers/frozen.tres",
            AcceleratePower => "res://CalyrexMod/icons/markers/accelerate.tres",
            SlowDownPower => "res://CalyrexMod/icons/markers/slow_down.tres",
            CannotMountPower => "res://CalyrexMod/icons/markers/cannot_mount.tres",
            LeechSeedPower => "res://CalyrexMod/icons/markers/leech_seed.tres",
            PressurePower => "res://CalyrexMod/icons/markers/pressure.tres",
            IngrainPower => "res://CalyrexMod/icons/markers/ingrain.tres",
            SoulHeartPower => "res://CalyrexMod/icons/markers/soul_heart.tres",
            TrickPower => "res://CalyrexMod/icons/markers/trick.tres",
            PPRestorePower => "res://CalyrexMod/icons/markers/pp_restore.tres",
            FriendlyGuardPower => "res://CalyrexMod/icons/markers/friendly_guard.tres",
            PlatedArmorPower => "res://CalyrexMod/icons/potions/plated_armor.tres",
            _ => null
        };
    }

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.PackedIconPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void PackedIconPathPostfix(PowerModel __instance, ref string __result)
    {
        string? fallback = GetFallbackIcon(__instance);
        if (fallback != null)
        {
            __result = fallback;
        }
    }

    [HarmonyPatch(typeof(PowerModel), "get_BigIconPath")]
    [HarmonyPostfix]
    private static void BigIconPathPostfix(PowerModel __instance, ref string __result)
    {
        string? fallback = GetFallbackIcon(__instance);
        if (fallback != null)
        {
            __result = fallback;
        }
    }
}
