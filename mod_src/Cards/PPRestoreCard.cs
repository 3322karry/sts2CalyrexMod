using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Cards;

public sealed class PPRestoreCard : CardModel
{
    public PPRestoreCard()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Energy", 4m);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<PPRestorePower>();
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PPRestorePower>(choiceContext, base.Owner.Creature, base.DynamicVars["Energy"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Energy"].UpgradeValueBy(1m);
    }
}
