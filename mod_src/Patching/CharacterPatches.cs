using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Entities.Players;
using CalyrexMod.Characters;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class CharacterPatches
{
    private static bool IsCalyrex(CharacterModel character)
    {
        return character is CalyrexCharacter;
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.TrailPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void TrailPathPostfix(CharacterModel __instance, ref string __result)
    {
        if (IsCalyrex(__instance))
        {
            __result = "res://scenes/vfx/card_trail_ironclad.tscn";
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "get_VisualsPath")]
    [HarmonyPostfix]
    private static void VisualsPathPostfix(CharacterModel __instance, ref string __result)
    {
        if (IsCalyrex(__instance))
        {
            __result = "res://CalyrexMod/scenes/calyrex.tscn";
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "get_IconTexturePath")]
    [HarmonyPostfix]
    private static void IconTexturePathPostfix(CharacterModel __instance, ref string __result)
    {
        if (IsCalyrex(__instance))
        {
            __result = "res://CalyrexMod/icons/calyrex_icon.tres";
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "get_IconTexture")]
    [HarmonyPostfix]
    private static void IconTexturePostfix(CharacterModel __instance, ref Godot.Texture2D __result)
    {
        if (IsCalyrex(__instance))
        {
            var tex = Godot.ResourceLoader.Load<Godot.Texture2D>("res://CalyrexMod/icons/calyrex_icon.tres", null, Godot.ResourceLoader.CacheMode.Reuse);
            __result = tex ?? __result;
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] IconTexture load: {(tex != null ? "OK" : "NULL")}");
        }
    }

    [HarmonyPatch(typeof(CharacterModel), "get_IconOutlineTexture")]
    [HarmonyPostfix]
    private static void IconOutlineTexturePostfix(CharacterModel __instance, ref Godot.Texture2D __result)
    {
        if (IsCalyrex(__instance))
        {
            var tex = Godot.ResourceLoader.Load<Godot.Texture2D>("res://CalyrexMod/icons/calyrex_icon.tres", null, Godot.ResourceLoader.CacheMode.Reuse);
            __result = tex ?? __result;
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.EnergyCounterPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void EnergyCounterPathPostfix(CharacterModel __instance, ref string __result)
    {
        if (IsCalyrex(__instance))
        {
            __result = "res://CalyrexMod/scenes/calyrex_energy_counter.tscn";
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.MerchantAnimPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void MerchantAnimPathPostfix(CharacterModel __instance, ref string __result)
    {
        if (IsCalyrex(__instance))
        {
            __result = "res://CalyrexMod/scenes/calyrex_merchant.tscn";
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.RestSiteAnimPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void RestSiteAnimPathPostfix(CharacterModel __instance, ref string __result)
    {
        if (IsCalyrex(__instance))
        {
            __result = "res://CalyrexMod/scenes/calyrex_rest_site.tscn";
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.CharacterSelectBg), MethodType.Getter)]
    [HarmonyPostfix]
    private static void CharacterSelectBgPostfix(CharacterModel __instance, ref string __result)
    {
        if (IsCalyrex(__instance))
        {
            __result = "res://CalyrexMod/scenes/char_select_bg_calyrex.tscn";
        }
    }

    [HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.CharacterSelectTransitionPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void CharacterSelectTransitionPathPostfix(CharacterModel __instance, ref string __result)
    {
        if (IsCalyrex(__instance))
        {
            __result = "res://materials/transitions/ironclad_transition_mat.tres";
        }
    }

    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCharacters), MethodType.Getter)]
    [HarmonyPostfix]
    private static void AllCharactersPostfix(ref IEnumerable<CharacterModel> __result)
    {
        try
        {
            __result = __result.Append(ModelDb.Character<CalyrexCharacter>());
        }
        catch (Exception)
        {
            // ModelDb may not be initialized yet; skip.
        }
    }

    // mod 角色没有注册 Epoch（官方角色的 {CHAR}{N}_EPOCH 不存在），赢 Boss 时游戏
    // ObtainCharUnlockEpoch 会 EpochModel.Get 崩溃。跳过 mod 角色的解锁纪元。
    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Saves.Managers.ProgressSaveManager), "ObtainCharUnlockEpoch")]
    [HarmonyPrefix]
    private static bool ObtainCharUnlockEpochPrefix(Player localPlayer)
    {
        try
        {
            if (localPlayer?.Character is CalyrexCharacter)
            {
                MegaCrit.Sts2.Core.Logging.Log.Info("[CalyrexMod] Skip char unlock epoch for CalyrexCharacter");
                return false;
            }
        }
        catch (Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] ObtainCharUnlockEpochPrefix: {ex}");
        }
        return true;
    }

    // CheckFifteenElitesDefeatedEpoch 对未知角色直接 throw（33121），mod 角色跳过
    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Saves.Managers.ProgressSaveManager), "CheckFifteenElitesDefeatedEpoch")]
    [HarmonyPrefix]
    private static bool CheckFifteenElitesDefeatedEpochPrefix(Player localPlayer)
    {
        try
        {
            if (localPlayer?.Character is CalyrexCharacter)
            {
                MegaCrit.Sts2.Core.Logging.Log.Info("[CalyrexMod] Skip fifteen elites epoch for CalyrexCharacter");
                return false;
            }
        }
        catch (Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] CheckFifteenElitesDefeatedEpochPrefix: {ex}");
        }
        return true;
    }

    // CheckFifteenBossesDefeatedEpoch 对未知角色直接 throw（33194），mod 角色跳过
    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Saves.Managers.ProgressSaveManager), "CheckFifteenBossesDefeatedEpoch")]
    [HarmonyPrefix]
    private static bool CheckFifteenBossesDefeatedEpochPrefix(Player localPlayer)
    {
        try
        {
            if (localPlayer?.Character is CalyrexCharacter)
            {
                MegaCrit.Sts2.Core.Logging.Log.Info("[CalyrexMod] Skip fifteen bosses epoch for CalyrexCharacter");
                return false;
            }
        }
        catch (Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] CheckFifteenBossesDefeatedEpochPrefix: {ex}");
        }
        return true;
    }
}
