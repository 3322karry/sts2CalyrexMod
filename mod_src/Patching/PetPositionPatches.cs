using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class PetPositionPatches
{
    private const float RightOffset = 90f;

    [HarmonyPatch(typeof(NCombatRoom), "AddCreature")]
    [HarmonyPostfix]
    private static void OnAddCreature(NCombatRoom __instance, MegaCrit.Sts2.Core.Entities.Creatures.Creature creature)
    {
        try
        {
            if (creature.PetOwner == null)
            {
                return;
            }
            var pets = creature.PetOwner.PlayerCombatState?.Pets;
            if (pets == null)
            {
                return;
            }
            foreach (var pet in pets)
            {
                var node = __instance.GetCreatureNode(pet);
                if (node == null)
                {
                    continue;
                }
                var pos = node.Position;
                pos.X += RightOffset;
                node.Position = pos;
            }
            Log.Info($"[CalyrexMod] PetPosition: pets shifted right by {RightOffset}");
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] PetPosition: {ex.Message}");
        }
    }
}
