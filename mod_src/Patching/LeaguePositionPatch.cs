using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using CalyrexMod.Monsters;

namespace CalyrexMod.Patching;

// 联赛怪物位置：按 slot 拉开距离并整体下移
[HarmonyPatch]
public static class LeaguePositionPatch
{
    [HarmonyPatch(typeof(NCombatRoom), "AddCreature")]
    [HarmonyPostfix]
    private static void OnAddCreature(NCombatRoom __instance, MegaCrit.Sts2.Core.Entities.Creatures.Creature creature)
    {
        try
        {
            if (creature.Monster is not LeagueMonsterBase)
            {
                return;
            }
            var node = __instance.GetCreatureNode(creature);
            if (node == null)
            {
                return;
            }
            var pos = node.Position;
            // 整体下移
            pos.Y += 120f;
            // 按槽位拉开
            switch (creature.SlotName)
            {
                case "slot1":
                    pos.X -= 160f;
                    break;
                case "slot2":
                    pos.X += 160f;
                    break;
            }
            node.Position = pos;
        }
        catch (System.Exception ex)
        {
            Log.Info($"[CalyrexMod] LeaguePosition: {ex.Message}");
        }
    }
}
