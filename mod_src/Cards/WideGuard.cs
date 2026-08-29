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

public sealed class WideGuard : CardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Buffer", 1m);
        }
    }

    public WideGuard()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 所有玩家获得一层缓冲
        if (base.CombatState == null)
        {
            return;
        }
        foreach (Player player in base.CombatState.Players)
        {
            await PowerCmd.Apply<BufferPower>(choiceContext, player.Creature, base.DynamicVars["Buffer"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Buffer"].UpgradeValueBy(1m);
    }
}
