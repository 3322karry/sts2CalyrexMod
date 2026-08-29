using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class Intimidate : CardModel
{
    protected override System.Collections.Generic.IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("StrDebuff", 2m);
        }
    }

    public Intimidate()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<FrozenPower>();
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await PowerCmd.Apply<StrengthPower>(choiceContext, cardPlay.Target, -base.DynamicVars["StrDebuff"].IntValue, base.Owner.Creature, this);
        await PowerCmd.Apply<FrozenPower>(choiceContext, cardPlay.Target, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级后：力量 -1（diff 绿色显示），不再消耗（卡面自动）
        base.DynamicVars["StrDebuff"].UpgradeValueBy(-1m);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
