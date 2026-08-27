using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class GlacialWorld : CardModel
{
    public GlacialWorld()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.FrozenTip;
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 所有冰冻翻倍
        if (base.CombatState == null)
        {
            return;
        }
        var frozenPowers = base.CombatState.Creatures
            .SelectMany((Creature c) => c.Powers.Where((PowerModel p) => p is FrozenPower))
            .ToList();
        foreach (var power in frozenPowers)
        {
            await PowerCmd.ModifyAmount(choiceContext, power, power.Amount, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        MockSetEnergyCost(new CardEnergyCost(this, 0, costsX: false));
    }
}
