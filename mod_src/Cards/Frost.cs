using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class Frost : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("PerFrost", 8m);
        }
    }

    public Frost()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override bool GainsBlock => true;

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.FrozenTip;
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 场上每有一层冰冻，获得 8 格挡
        int frostTotal = 0;
        if (base.CombatState != null)
        {
            foreach (var creature in base.CombatState.Creatures)
            {
                frostTotal += creature.Powers.Where((PowerModel p) => p is FrozenPower).Sum((PowerModel p) => p.Amount);
            }
        }
        decimal block = frostTotal * base.DynamicVars["PerFrost"].BaseValue;
        await CreatureCmd.GainBlock(base.Owner.Creature, block, ValueProp.Unpowered, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PerFrost"].UpgradeValueBy(3m);
    }
}
