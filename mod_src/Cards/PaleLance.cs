using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Cards;

public sealed class PaleLance : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Energy", 0m);
        }
    }

    public PaleLance()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromCard<GlacialLance>();
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        // 洗入一张雪矛到抽牌堆
        var lance = combatState.CreateCard<GlacialLance>(base.Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(new[] { lance }, PileType.Draw, base.Owner, CardPilePosition.Random));

        // 升级所有雪矛（任何牌堆/手牌）
        var piles = new[] { PileType.Draw, PileType.Hand, PileType.Discard, PileType.Exhaust };
        foreach (var pileType in piles)
        {
            var pile = pileType.GetPile(base.Owner);
            foreach (var card in pile.Cards.Where((CardModel c) => c is GlacialLance).ToList())
            {
                CardCmd.Upgrade(card);
            }
        }

        // 无论何处，将雪矛都加入手牌
        foreach (var pileType in piles)
        {
            var pile = pileType.GetPile(base.Owner);
            var lances = pile.Cards.Where((CardModel c) => c is GlacialLance).ToList();
            if (lances.Count > 0)
            {
                foreach (var card in lances)
                {
                    await CardPileCmd.Add(card, PileType.Hand);
                }
            }
        }

        // 升级后额外获得 2 费
        int energy = base.DynamicVars["Energy"].IntValue;
        if (energy > 0)
        {
            await PlayerCmd.GainEnergy(energy, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Energy"].UpgradeValueBy(2m);
    }
}
