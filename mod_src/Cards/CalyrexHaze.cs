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

public sealed class CalyrexHaze : CardModel
{
    public CalyrexHaze()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 将手牌中任意张牌变化为"归零"
        var hand = PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c != this && c.IsTransformable).ToList();
        if (hand.Count == 0)
        {
            return;
        }
        var prefs = new CardSelectorPrefs(new LocString("cards", "HAZE.selectionPrompt"), 1, hand.Count);
        var chosen = (await CardSelectCmd.FromHand(choiceContext, base.Owner, prefs, (CardModel c) => c != this && c.IsTransformable, this)).ToList();
        foreach (var card in chosen)
        {
            await CardCmd.TransformTo<Zero>(card);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：归零+（Zero 自动升级）
    }
}
