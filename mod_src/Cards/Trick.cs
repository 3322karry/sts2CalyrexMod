using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class Trick : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Cards", 2m);
        }
    }

    public Trick()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 抽 2 张牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars["Cards"].BaseValue, base.Owner);

        // 本回合 -1 力 -1 敏
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, -1m, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, -1m, base.Owner.Creature, this);
        await PowerCmd.Apply<TrickPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
