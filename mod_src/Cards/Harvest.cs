using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class Harvest : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Percent", 25m);
            yield return new IntVar("PerLayer", 3m);
        }
    }

    public Harvest()
        : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.AbundanceTip;
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 消耗 25% 丰饶（向上取整），伤害 = 消耗层数 × 3
        var abundancePower = base.Owner.Creature.Powers.FirstOrDefault((PowerModel p) => p is Abundance);
        int total = abundancePower?.Amount ?? 0;
        if (total <= 0)
        {
            return;
        }
        int consumed = (int)System.Math.Ceiling(total * base.DynamicVars["Percent"].BaseValue / 100m);
        consumed = System.Math.Max(1, consumed);

        if (abundancePower != null)
        {
            await PowerCmd.ModifyAmount(choiceContext, abundancePower, -consumed, base.Owner.Creature, this);
        }

        decimal damage = consumed * base.DynamicVars["PerLayer"].BaseValue;
        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Percent"].UpgradeValueBy(25m);
        base.DynamicVars["PerLayer"].UpgradeValueBy(1m);
    }
}
