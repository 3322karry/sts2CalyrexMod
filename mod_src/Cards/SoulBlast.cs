using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Cards;

public sealed class SoulBlast : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("HpCost", 10m);
            yield return new IntVar("Energy", 5m);
        }
    }

    public SoulBlast()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 失去 X 血量（不可格挡伤害）
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars["HpCost"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, this);

        // 获得能量
        await PlayerCmd.GainEnergy(base.DynamicVars["Energy"].BaseValue, base.Owner);

        // 抽牌直到手牌满
        int missing = CardPile.MaxCardsInHand - PileType.Hand.GetPile(base.Owner).Cards.Count;
        if (missing > 0)
        {
            await CardPileCmd.Draw(choiceContext, missing, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HpCost"].UpgradeValueBy(-2m);
    }
}
