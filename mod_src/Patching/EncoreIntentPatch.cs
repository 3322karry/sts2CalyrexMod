using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using CalyrexMod.Powers;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class EncoreIntentPatch
{
    // 玩家回合准备敌人意图时（PrepareForNextTurn → RollMove + RefreshIntents 之后），
    // 若敌人带"再来一次"则把意图锁定为已保存的行动，并刷新 UI。
    [HarmonyPatch(typeof(Creature), "PrepareForNextTurn")]
    [HarmonyPostfix]
    private static void PrepareForNextTurnPostfix(Creature __instance)
    {
        try
        {
            var encore = __instance.Powers.FirstOrDefault((PowerModel p) => p is EncorePower) as EncorePower;
            if (encore == null || !encore.HasLockedMove())
            {
                return;
            }
            if (__instance.Monster?.MoveStateMachine == null)
            {
                return;
            }
            var machine = __instance.Monster.MoveStateMachine;
            string lockedId = encore.LockedStateId;
            if (lockedId == null || !machine.States.ContainsKey(lockedId))
            {
                return;
            }
            if (machine.States[lockedId] is MoveState lockedState)
            {
                __instance.Monster.SetMoveImmediate(lockedState, forceTransition: true);
                MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Encore intent locked: {lockedId}");
            }
        }
        catch (System.Exception ex)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"[CalyrexMod] EncoreIntentPatch failed: {ex}");
        }
    }
}
