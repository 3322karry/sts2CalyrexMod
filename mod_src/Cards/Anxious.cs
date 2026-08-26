using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Cards;

public sealed class Anxious : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("HpCost", 8m);
        }
    }

    public Anxious()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.MountTip;
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 受到 8 点伤害
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars["HpCost"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, this);

        // 骑马
        await MountHelper.DoMount(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HpCost"].UpgradeValueBy(-2m);
    }
}
