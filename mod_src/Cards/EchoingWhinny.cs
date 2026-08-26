using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Cards;

public sealed class EchoingWhinny : CardModel
{
    public EchoingWhinny()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(base.Owner);
        var handPrefs = new CardSelectorPrefs(new LocString("cards", "ECHOING_WHINNY.handSelectionPrompt"), 1);
        var handCard = (await CardSelectCmd.FromHand(choiceContext, base.Owner, handPrefs, (CardModel c) => c != this && c.IsRemovable && c.Type != CardType.Curse, this)).FirstOrDefault();
        if (handCard != null)
        {
            handCard.BaseReplayCount += 2;
            handCard.AddKeyword(CardKeyword.Exhaust);
        }

        var draw = PileType.Draw.GetPile(base.Owner);
        var drawPrefs = new CardSelectorPrefs(new LocString("cards", "ECHOING_WHINNY.drawSelectionPrompt"), 1);
        var drawCard = (await CardSelectCmd.FromCombatPile(choiceContext, draw, base.Owner, drawPrefs, (CardModel c) => c.IsRemovable && c.Type != CardType.Curse)).FirstOrDefault();
        if (drawCard != null)
        {
            drawCard.BaseReplayCount += 2;
            drawCard.AddKeyword(CardKeyword.Exhaust);
        }
    }

    protected override void OnUpgrade()
    {
        MockSetEnergyCost(new CardEnergyCost(this, 1, costsX: false));
    }
}
