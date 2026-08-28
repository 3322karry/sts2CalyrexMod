using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Monsters;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class PlantCarrot : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Abundance", 2m);
            yield return new IntVar("Feed", 4m);
        }
    }

    public PlantCarrot()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.AbundanceTip;
            yield return KeywordTipHelper.QuickSightTip;
            yield return KeywordTipHelper.HeavyLanceTip;
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.Owner.PlayerCombatState;
        if (combatState == null || base.Owner.Creature.CombatState == null)
        {
            return;
        }

        // 丰饶 2
        await PowerCmd.Apply<Abundance>(choiceContext, base.Owner.Creature, base.DynamicVars["Abundance"].BaseValue, base.Owner.Creature, this);

        // 喂养 4
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

        // 选择一匹马，增加一层标记（灵幽马→迅疾之视，雪暴马→重装之矛）
        var choices = new List<CardModel>();
        if (glastrier != null && glastrier.IsAlive)
        {
            choices.Add(base.Owner.Creature.CombatState.CreateCard<MountChoiceGlastrier>(base.Owner));
        }
        if (spectrier != null && spectrier.IsAlive)
        {
            choices.Add(base.Owner.Creature.CombatState.CreateCard<MountChoiceSpectrier>(base.Owner));
        }
        if (choices.Count == 0)
        {
            return;
        }

        CardModel? chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, choices, base.Owner, canSkip: true);
        if (chosen == null)
        {
            return;
        }

        if (chosen is MountChoiceGlastrier)
        {
            await PowerCmd.Apply<HeavyLance>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
        }
        else if (chosen is MountChoiceSpectrier)
        {
            await PowerCmd.Apply<QuickSight>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Abundance"].UpgradeValueBy(1m);
        base.DynamicVars["Feed"].UpgradeValueBy(2m);
    }
}
