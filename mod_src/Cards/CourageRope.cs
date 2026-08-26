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

public sealed class CourageRope : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Feed", 4m);
        }
    }

    public CourageRope()
        : base(2, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

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
        // 击晕一名敌人
        if (cardPlay.Target != null)
        {
            await CreatureCmd.Stun(cardPlay.Target, (_) => Task.CompletedTask);
        }

        // 喂养 + 骑马
        var combatState = base.Owner.PlayerCombatState;
        if (combatState != null)
        {
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
        }
        await MountHelper.DoMount(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Feed"].UpgradeValueBy(2m);
        AddKeyword(CardKeyword.Innate);
        AddKeyword(CardKeyword.Retain);
    }
}
