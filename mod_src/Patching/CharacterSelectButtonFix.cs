using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Characters;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class CharacterSelectButtonFix
{
    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NCharacterSelectButton), "Init", new[] { typeof(CharacterModel), typeof(MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.ICharacterSelectButtonDelegate) })]
    [HarmonyPostfix]
    private static void InitPostfix(MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NCharacterSelectButton __instance, CharacterModel character)
    {
        try
        {
            if (character is not CalyrexCharacter)
            {
                return;
            }
            var icon = HarmonyLib.Traverse.Create(__instance).Field("_icon").GetValue() as TextureRect;
            if (icon == null)
            {
                return;
            }
            var tex = ResourceLoader.Load<Texture2D>("res://CalyrexMod/icons/char_select_calyrex.tres", null, ResourceLoader.CacheMode.Reuse);
            if (tex != null)
            {
                icon.Texture = tex;
                Log.Info("[CalyrexMod] CharSelectButtonFix: applied Calyrex icon");
            }
        }
        catch (System.Exception ex)
        {
            Log.Info($"[CalyrexMod] CharSelectButtonFix: {ex}");
        }
    }
}
