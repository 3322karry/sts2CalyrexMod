using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Cards;

public sealed class Accelerate : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new EnergyVar(1);
            yield return new IntVar("Strength", 1m);
        }
    }

    public Accelerate()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<AcceleratePower>();            yield return KeywordTipHelper.MountTip;
            yield return HoverTipFactory.FromPower<StrengthPower>();

        }
    }

    // 只有骑灵幽马（黑马）时才能使用
    protected override bool IsPlayable => base.Owner != null && base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedSpectrier);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 每使用 3 费，获得 1 费 1 力（升级后 2 力）
        int layers = base.DynamicVars["Strength"].IntValue;
        await PowerCmd.Apply<AcceleratePower>(choiceContext, base.Owner.Creature, layers, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Strength"].UpgradeValueBy(1m);
    }
}
