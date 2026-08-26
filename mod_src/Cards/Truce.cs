using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class Truce : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("PreAbundance", 0m);
        }
    }

    public Truce()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<TrucePower>();
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Truce played: IsUpgraded={base.IsUpgraded} PreAbundance={base.DynamicVars["PreAbundance"].IntValue}");

        // 升级：先获得 3 丰饶
        if (base.IsUpgraded)
        {
            await PowerCmd.Apply<Abundance>(choiceContext, base.Owner.Creature, 3m, base.Owner.Creature, this);
        }

        // 丰饶翻倍 + 等量于翻倍后层数的格挡
        var abundancePower = base.Owner.Creature.Powers.FirstOrDefault((PowerModel p) => p is Abundance);
        int amount = abundancePower?.Amount ?? 0;
        if (abundancePower != null && amount > 0)
        {
            await PowerCmd.ModifyAmount(choiceContext, abundancePower, amount, base.Owner.Creature, this);
            await CreatureCmd.GainBlock(base.Owner.Creature, amount * 2, ValueProp.Unpowered, cardPlay);
        }

        // 本回合不能再打出攻击牌
        await PowerCmd.Apply<TrucePower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PreAbundance"].UpgradeValueBy(3m);
    }
}
