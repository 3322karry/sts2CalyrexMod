using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using CalyrexMod.Monsters;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public static class MountHelper
{
    public static async Task DoMount(PlayerChoiceContext choiceContext, CardModel source)
    {
        await DoMount(choiceContext, source.Owner, source);
    }

    public static async Task DoMount(PlayerChoiceContext choiceContext, Player owner, CardModel? source)
    {
        var combatState = owner.PlayerCombatState;
        if (combatState == null || owner.Creature.CombatState == null)
        {
            return;
        }

        // 本次战斗无法再骑马
        if (owner.Creature.Powers.Any((PowerModel p) => p is CannotMountPower))
        {
            return;
        }

        bool mountedGlastrier = owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier);
        bool mountedSpectrier = owner.Creature.Powers.Any((PowerModel p) => p is MountedSpectrier);

        var choices = new List<CardModel>();
        Creature? liveGlastrier = combatState.GetPet<Glastrier>();
        if (liveGlastrier != null && liveGlastrier.IsAlive)
        {
            choices.Add(owner.Creature.CombatState.CreateCard<MountChoiceGlastrier>(owner));
        }
        Creature? liveSpectrier = combatState.GetPet<Spectrier>();
        if (liveSpectrier != null && liveSpectrier.IsAlive)
        {
            choices.Add(owner.Creature.CombatState.CreateCard<MountChoiceSpectrier>(owner));
        }
        if (mountedGlastrier || mountedSpectrier)
        {
            choices.Add(owner.Creature.CombatState.CreateCard<MountChoiceUnmount>(owner));
        }

        if (choices.Count == 0)
        {
            return;
        }

        CardModel? chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, choices, owner, canSkip: true);
        if (chosen == null)
        {
            return;
        }

        if (chosen is MountChoiceUnmount)
        {
            await DoUnmount(choiceContext, owner);
            return;
        }

        if (mountedGlastrier)
        {
            await DoUnmount(choiceContext, owner);
        }
        if (mountedSpectrier)
        {
            await DoUnmount(choiceContext, owner);
        }

        Creature? steed = chosen switch
        {
            MountChoiceGlastrier => combatState.GetPet<Glastrier>(),
            MountChoiceSpectrier => combatState.GetPet<Spectrier>(),
            _ => null
        };
        if (steed == null || !steed.IsAlive)
        {
            return;
        }

        await PowerCmd.Remove<SteedGuard>(steed);

        int shuffleCount = 0;
        int heavyLanceLayers = 0;
        bool isSpectrier = steed.Monster is Spectrier;
        if (isSpectrier)
        {
            var qs = steed.Powers.FirstOrDefault((PowerModel p) => p is QuickSight);
            shuffleCount = qs?.Amount ?? 0;
        }
        else
        {
            var hl = steed.Powers.FirstOrDefault((PowerModel p) => p is HeavyLance);
            shuffleCount = hl?.Amount ?? 0;
            heavyLanceLayers = hl?.Amount ?? 0;
        }

        if (steed.Monster is Glastrier)
        {
            await PowerCmd.Apply<MountedGlastrier>(choiceContext, owner.Creature, steed.MaxHp, owner.Creature, source);
            if (heavyLanceLayers > 0)
            {
                await PowerCmd.Apply<IceWallPower>(choiceContext, owner.Creature, heavyLanceLayers, owner.Creature, source);
            }
        }
        else
        {
            await PowerCmd.Apply<MountedSpectrier>(choiceContext, owner.Creature, steed.MaxHp, owner.Creature, source);
        }
        // 合体标记：防止马的死亡触发魂心等死亡效果
        await PowerCmd.Apply<MountMergePower>(choiceContext, steed, 1m, owner.Creature, source);
        await CreatureCmd.Kill(steed, force: true);
        await PowerCmd.Apply<EternalWhinny>(choiceContext, owner.Creature, 1m, owner.Creature, source);

        CalyrexMod.Patching.MountedVisualPatches.ApplyVisual(owner, isSpectrier
            ? CalyrexMod.Patching.MountedVisualPatches.MountedSpectrierVisualPath
            : CalyrexMod.Patching.MountedVisualPatches.MountedGlastrierVisualPath);

        // 每层标记洗入一张对应招式卡
        var combatState2 = owner.Creature.CombatState;
        if (combatState2 != null && shuffleCount > 0)
        {
            var cards = new List<CardModel>();
            for (int i = 0; i < shuffleCount; i++)
            {
                cards.Add(isSpectrier
                    ? (CardModel)combatState2.CreateCard<AstralBarrage>(owner)
                    : combatState2.CreateCard<GlacialLance>(owner));
            }
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, owner, CardPilePosition.Random));
        }

        // 单敌战斗（战斗开始只有 1 个敌人）：合体瞬间给 2 虚弱 2 易伤
        var encounter = owner.Creature.CombatState?.Encounter;
        if (encounter != null && encounter.MonstersWithSlots.Count == 1)
        {
            var loneEnemy = owner.Creature.CombatState?.Enemies.FirstOrDefault((Creature e) => e.IsAlive);
            if (loneEnemy != null)
            {
                await PowerCmd.Apply<WeakPower>(choiceContext, loneEnemy, 2m, owner.Creature, source);
                await PowerCmd.Apply<VulnerablePower>(choiceContext, loneEnemy, 2m, owner.Creature, source);
            }
        }
    }

    public static async Task DoUnmount(PlayerChoiceContext choiceContext, CardModel source)
    {
        await DoUnmount(choiceContext, source.Owner);
    }

    public static async Task DoUnmount(PlayerChoiceContext choiceContext, Player owner)
    {
        var combatState = owner.PlayerCombatState;
        if (combatState == null)
        {
            return;
        }

        var mountedPowers = owner.Creature.Powers.Where((PowerModel p) => p is MountedGlastrier || p is MountedSpectrier).ToList();
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] MountHelper.Unmount: {string.Join(",", mountedPowers.Select((PowerModel p) => p.GetType().Name + "=" + p.Amount))}");

        await PowerCmd.Remove<EternalWhinny>(owner.Creature);
        await PowerCmd.Remove<IceWallPower>(owner.Creature);

        CalyrexMod.Patching.MountedVisualPatches.ApplyVisual(owner, CalyrexMod.Patching.MountedVisualPatches.NormalVisualPath);

        if (owner.Creature.Powers.FirstOrDefault((PowerModel p) => p is MountedGlastrier) is PowerModel mg)
        {
            await PowerCmd.Remove<MountedGlastrier>(owner.Creature);
            Creature glastrier = await PlayerCmd.AddPet<Glastrier>(owner);
            if (mg.Amount > 0m)
            {
                await CreatureCmd.GainMaxHp(glastrier, mg.Amount);
            }
            await PowerCmd.Apply<HeavyLance>(new ThrowingPlayerChoiceContext(), glastrier, 1m, owner.Creature, null, silent: true);
        }
        if (owner.Creature.Powers.FirstOrDefault((PowerModel p) => p is MountedSpectrier) is PowerModel ms)
        {
            await PowerCmd.Remove<MountedSpectrier>(owner.Creature);
            Creature spectrier = await PlayerCmd.AddPet<Spectrier>(owner);
            if (ms.Amount > 0m)
            {
                await CreatureCmd.GainMaxHp(spectrier, ms.Amount);
            }
            await PowerCmd.Apply<QuickSight>(new ThrowingPlayerChoiceContext(), spectrier, 1m, owner.Creature, null, silent: true);
        }
    }

        // 喂养工具：两匹马各 +X 最大生命；马死亡/不在场时自动复活再喂养
    public static async Task FeedBoth(PlayerChoiceContext choiceContext, Player owner, decimal amount)
    {
        // 合体中的马（MountedGlastrier/Spectrier）不算宠物：不召唤、不复活，只喂在场的那匹
        bool gMounted = owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier);
        bool sMounted = owner.Creature.Powers.Any((PowerModel p) => p is MountedSpectrier);
        if (!gMounted)
        {
            await FeedOne(choiceContext, owner, amount, preferred: typeof(Glastrier));
        }
        if (!sMounted)
        {
            await FeedOne(choiceContext, owner, amount, preferred: typeof(Spectrier));
        }
    }

    private static async Task FeedOne(PlayerChoiceContext choiceContext, Player owner, decimal amount, Type preferred)
    {
        var combatState = owner.PlayerCombatState;
        if (combatState == null)
        {
            return;
        }
        Creature? steed = preferred == typeof(Glastrier) ? combatState.GetPet<Glastrier>() : combatState.GetPet<Spectrier>();
        string steedName = preferred == typeof(Glastrier) ? "Glastrier" : "Spectrier";
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] FeedOne({steedName}): steed={(steed == null ? "null" : (steed.IsAlive ? "alive" : "DEAD"))} hp={steed?.CurrentHp}/{steed?.MaxHp}");
        // 优先喂 preferred 马；若它活着则直接喂
        if (steed != null && steed.IsAlive)
        {
            await CreatureCmd.GainMaxHp(steed, amount);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] FeedOne({steedName}): fed alive, hp={steed.CurrentHp}/{steed.MaxHp}");
            return;
        }
        // preferred 马死了：直接 GainMaxHp（内部 Heal 会复活死马）
        if (steed != null && steed.IsDead)
        {
            await CreatureCmd.GainMaxHp(steed, amount);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] FeedOne({steedName}): revived dead, hp={steed.CurrentHp}/{steed.MaxHp} alive={steed.IsAlive}");
            return;
        }
        // preferred 马不在场：召唤它再喂（两匹都不在场时依次召唤）
        if (preferred == typeof(Glastrier))
        {
            var g = await PlayerCmd.AddPet<Glastrier>(owner);
            await CreatureCmd.GainMaxHp(g, amount);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] FeedOne({steedName}): AddPet spawn, hp={g.CurrentHp}/{g.MaxHp} alive={g.IsAlive}");
        }
        else
        {
            var sp = await PlayerCmd.AddPet<Spectrier>(owner);
            await CreatureCmd.GainMaxHp(sp, amount);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] FeedOne({steedName}): AddPet spawn, hp={sp.CurrentHp}/{sp.MaxHp} alive={sp.IsAlive}");
        }
    }
}
