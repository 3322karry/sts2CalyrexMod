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

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }
        // 从4张随机消耗牌中选一张加入手牌（本回合可免费打出）
        var candidates = ModelDb.AllCards
            .Where((CardModel c) => c.Keywords.Contains(CardKeyword.Exhaust) && c.CanBeGeneratedInCombat && c.Type != CardType.Curse)
            .ToList();
        if (candidates.Count == 0)
        {
            return;
        }
        var rng = base.Owner.PlayerRng.Rewards;
        var options = new List<CardModel>();
        var pool = new List<CardModel>(candidates);
        int want = base.DynamicVars["Options"].IntValue;
        while (options.Count < want && pool.Count > 0)
        {
            int idx = rng.NextInt(pool.Count);
            options.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        if (options.Count == 0)
        {
            return;
        }
        var choiceCards = options.Select((CardModel c) => combatState.CreateCard(c, base.Owner)).ToList();
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
