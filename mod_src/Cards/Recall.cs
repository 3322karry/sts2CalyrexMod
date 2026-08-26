using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Cards;

public sealed class Recall : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new BlockVar(3m, ValueProp.Move);
        }
    }

    public Recall()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override bool GainsBlock => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        // 自选一张消耗牌堆的牌，复制加入手牌
        var exhaustPile = PileType.Exhaust.GetPile(base.Owner);
        var candidates = exhaustPile.Cards.Where((CardModel c) => c.IsRemovable).ToList();
        if (candidates.Count == 0)
        {
            return;
        }
        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
        CardModel chosen = (await CardSelectCmd.FromCombatPile(choiceContext, exhaustPile, base.Owner, prefs, (CardModel c) => c.IsRemovable)).FirstOrDefault();
        if (chosen == null)
        {
            return;
        }
        var copy = combatState.CloneCard(chosen);
        await CardPileCmd.AddGeneratedCardsToCombat(new[] { copy }, PileType.Hand, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}
