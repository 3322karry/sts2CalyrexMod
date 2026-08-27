using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Monsters;

namespace CalyrexMod.Cards;

public sealed class SeedBomb : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DamageVar(5m, ValueProp.Move);
            yield return new IntVar("Feed", 2m);
        }
    }

    public SeedBomb()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 造成 5 伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 喂养 2
        var combatState = base.Owner.PlayerCombatState;
        if (combatState == null)
        {
            return;
        }
        decimal feed = base.DynamicVars["Feed"].BaseValue;
        Creature? glastrier = combatState.GetPet<Glastrier>();
        if (glastrier != null)
        {
            await CreatureCmd.GainMaxHp(glastrier, feed);
        }
        Creature? spectrier = combatState.GetPet<Spectrier>();
        if (spectrier != null)
        {
            await CreatureCmd.GainMaxHp(spectrier, feed);
        }
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
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["Feed"].UpgradeValueBy(2m);
    }
}
