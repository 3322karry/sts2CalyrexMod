using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CalyrexMod.Cards;

public sealed class MoveRecord : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Options", 4m);
        }
    }

    public MoveRecord()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override void OnUpgrade()
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }
        // 从玩家角色卡池的消耗牌中随机选 4 张（排除先古/无色/调试卡）
        var pool = base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
            .Where((CardModel c) => c.Keywords.Contains(CardKeyword.Exhaust) && c.CanBeGeneratedInCombat && c.Type != CardType.Curse
                && c.Rarity != CardRarity.Ancient
                && c.Pool is not ColorlessCardPool
                && c is not DebugCard)
            .Distinct()
            .ToList();
        if (pool.Count == 0)
        {
            return;
        }
        var rng = base.Owner.PlayerRng.Rewards;
        var options = new List<CardModel>();
        var candidates = new List<CardModel>(pool);
        int want = base.DynamicVars["Options"].IntValue;
        while (options.Count < want && candidates.Count > 0)
        {
            int idx = rng.NextInt(candidates.Count);
            options.Add(candidates[idx]);
            candidates.RemoveAt(idx);
        }
        if (options.Count == 0)
        {
            return;
        }
        var choiceCards = options.Select((CardModel c) =>
        {
            var card = combatState.CreateCard(c, base.Owner);
            // 升级后：选项为升级版消耗牌
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(card);
            }
            return card;
        }).ToList();
        var prefs = new CardSelectorPrefs(new LocString("cards", "MOVE_RECORD.selectionPrompt"), 1);
        var chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, choiceCards, base.Owner, prefs)).FirstOrDefault();
        if (chosen == null)
        {
            return;
        }
        chosen.EnergyCost.SetThisCombat(0);
        await CardPileCmd.AddGeneratedCardsToCombat(new[] { chosen }, PileType.Hand, base.Owner);
    }
}
