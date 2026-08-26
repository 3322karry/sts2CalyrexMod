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

        // 喂养：两匹马各 +X 最大生命（战死的马也会被喂养复活；已合体的马不在场则跳过）
        decimal feedAmount = base.DynamicVars["Feed"].IntValue;
        var glastrier = combatState.GetPet<Glastrier>();
        if (glastrier != null)
        {
            await CreatureCmd.GainMaxHp(glastrier, feedAmount);
        }
        var spectrier = combatState.GetPet<Spectrier>();
        if (spectrier != null)
        {
            await CreatureCmd.GainMaxHp(spectrier, feedAmount);
        }

        // 骑马
        await MountHelper.DoMount(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Feed"].UpgradeValueBy(2m);
        AddKeyword(CardKeyword.Retain);
    }
}
