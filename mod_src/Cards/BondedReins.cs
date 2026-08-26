using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Monsters;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class BondedReins : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Feed", 4m);
        }
    }

    public BondedReins()
        : base(2, CardType.Skill, CardRarity.Basic, TargetType.None)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return new HoverTip(new LocString("cards", "KEYWORD_FEED.title"), new LocString("cards", "KEYWORD_FEED.description"));
            yield return new HoverTip(new LocString("cards", "KEYWORD_MOUNT.title"), new LocString("cards", "KEYWORD_MOUNT.description"));
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.Owner.PlayerCombatState;
        if (combatState == null)
        {
            return;
        }

        // 喂养：两匹马各 +X 最大生命（死马自动复活）
        decimal feedAmount = base.DynamicVars["Feed"].IntValue;
        await MountHelper.FeedBoth(choiceContext, base.Owner, feedAmount);

        // 骑马
        await MountHelper.DoMount(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Feed"].UpgradeValueBy(2m);
        AddKeyword(CardKeyword.Retain);
    }
}
