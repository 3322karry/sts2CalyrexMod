using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Relics;

namespace CalyrexMod.Cards;

public sealed class WishGrant : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Energy", 1m);
        }
    }

    public WishGrant()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Retain };

    // 首次抽到不能打出；若上回合这张牌被保留（在上回合手牌中），可以打出
    protected override bool IsPlayable
    {
        get
        {
            if (base.Owner == null)
            {
                return false;
            }
            var carrot = base.Owner.GetRelic<BlackWhiteCarrot>();
            return carrot != null && carrot.WasInHandLastTurn(this);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得 1 费
        await PlayerCmd.GainEnergy(base.DynamicVars["Energy"].BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Energy"].UpgradeValueBy(1m);
    }
}
