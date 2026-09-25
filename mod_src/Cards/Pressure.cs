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

public sealed class Pressure : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Vuln", 1m);
        }
    }

    public Pressure()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<PressurePower>();            yield return HoverTipFactory.FromPower<VulnerablePower>();

        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 升级后每次攻击获得 2 层易伤
        int layers = base.DynamicVars["Vuln"].IntValue;
        await PowerCmd.Apply<PressurePower>(choiceContext, base.Owner.Creature, layers, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Vuln"].UpgradeValueBy(1m);
    }
}
