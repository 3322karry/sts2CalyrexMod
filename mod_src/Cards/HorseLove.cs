using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using CalyrexMod.Monsters;

namespace CalyrexMod.Cards;

public sealed class HorseLove : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Feed", 14m);
        }
    }

    public HorseLove()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.Owner.PlayerCombatState;
        if (combatState == null)
        {
            return;
        }
        decimal feed = base.DynamicVars["Feed"].BaseValue;
        await MountHelper.FeedBoth(choiceContext, base.Owner, feed);
    }

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.FeedTip;
        }
    }

protected override void OnUpgrade()
    {
        base.DynamicVars["Feed"].UpgradeValueBy(4m);
    }
}
