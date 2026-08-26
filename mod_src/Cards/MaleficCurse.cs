using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class MaleficCurse : CardModel
{
    public MaleficCurse()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

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

        // 失去一半血量（当前血量）
        decimal hpLoss = base.Owner.Creature.CurrentHp / 2m;
        if (hpLoss > 0m)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, hpLoss, ValueProp.Unblockable | ValueProp.Unpowered, null, this);
        }

        // 敌人获得等量于丰饶层数的灾厄
        int abundance = base.Owner.Creature.Powers.FirstOrDefault((PowerModel p) => p is Abundance)?.Amount ?? 0;
        if (abundance > 0)
        {
            await PowerCmd.Apply<DoomPower>(choiceContext, cardPlay.Target, abundance, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        MockSetEnergyCost(new CardEnergyCost(this, 1, costsX: false));
    }
}
