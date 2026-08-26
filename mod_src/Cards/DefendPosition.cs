using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class DefendPosition : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("PerCard", 10m);
        }
    }

    public DefendPosition()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public override bool GainsBlock => true;

    // 只有骑雪暴马（白马）时才能使用
    protected override bool IsPlayable => base.Owner != null && base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier);

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.MountedGlastrierTip;
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 手牌每有一张牌，获得 10 格挡
        int count = PileType.Hand.GetPile(base.Owner).Cards.Count;
        decimal block = count * base.DynamicVars["PerCard"].BaseValue;
        await CreatureCmd.GainBlock(base.Owner.Creature, block, ValueProp.Unpowered, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PerCard"].UpgradeValueBy(5m);
    }
}
