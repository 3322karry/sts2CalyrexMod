using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Cards;

public sealed class LongHowl : CardModel
{

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Strength", 2m);
        }
    }

    public LongHowl()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 所有玩家加 2 力量
        if (base.CombatState == null)
        {
            return;
        }
        foreach (Player player in base.CombatState.Players)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, base.DynamicVars["Strength"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Strength"].UpgradeValueBy(1m);
    }
}
