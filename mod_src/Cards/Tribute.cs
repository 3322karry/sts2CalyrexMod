using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Monsters;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class Tribute : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("FeedPer", 2m);
        }
    }

    public Tribute()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<Abundance>();
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 失去所有丰饶
        var abundancePower = base.Owner.Creature.Powers.FirstOrDefault((PowerModel p) => p is Abundance);
        int lost = abundancePower?.Amount ?? 0;
        if (abundancePower != null)
        {
            await PowerCmd.Remove<Abundance>(base.Owner.Creature);
        }

        // 每失去 1 点丰饶，喂养 X
        int feedPer = base.DynamicVars["FeedPer"].IntValue;
        decimal feedAmount = lost * feedPer;

        var combatState = base.Owner.PlayerCombatState;
        if (combatState == null || feedAmount <= 0)
        {
            return;
        }

        Creature? glastrier = combatState.GetPet<Glastrier>();
        if (glastrier != null)
        {
            await CreatureCmd.GainMaxHp(glastrier, feedAmount);
        }
        Creature? spectrier = combatState.GetPet<Spectrier>();
        if (spectrier != null)
        {
            await CreatureCmd.GainMaxHp(spectrier, feedAmount);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["FeedPer"].UpgradeValueBy(1m);
    }
}
