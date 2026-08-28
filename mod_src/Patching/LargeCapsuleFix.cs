using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using CalyrexMod.Characters;
using CalyrexMod.Cards;

namespace CalyrexMod.Patching;

// 涅奥"大胶囊"遗物：蕾冠王角色返回专属打击/防御（官方 First 查询找不到时卡死）
[HarmonyPatch]
public static class LargeCapsuleFix
{
    [HarmonyPatch(typeof(LargeCapsule), "GetStrikeForCharacter")]
    [HarmonyPrefix]
    private static bool GetStrikePrefix(CharacterModel character, ref CardModel __result)
    {
        if (character is CalyrexCharacter)
        {
            __result = ModelDb.Card<CalyrexStrike>();
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(LargeCapsule), "GetDefendForCharacter")]
    [HarmonyPrefix]
    private static bool GetDefendPrefix(CharacterModel character, ref CardModel __result)
    {
        if (character is CalyrexCharacter)
        {
            __result = ModelDb.Card<CalyrexDefend>();
            return false;
        }
        return true;
    }
}
