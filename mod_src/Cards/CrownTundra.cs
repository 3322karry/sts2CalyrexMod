using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class CrownTundra : CardModel
{
    public CrownTundra()
        : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.AbundanceTip;
            yield return KeywordTipHelper.FrozenTip;
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人施加等量于丰饶层数的冰冻
        int abundance = base.Owner.Creature.Powers.FirstOrDefault((PowerModel p) => p is Abundance)?.Amount ?? 0;
        if (abundance <= 0)
        {
            return;
        }
        foreach (var enemy in base.CombatState.Enemies.Where((Creature e) => e.IsAlive))
        {
            await PowerCmd.Apply<FrozenPower>(choiceContext, enemy, abundance, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后费用不变
        AddKeyword(CardKeyword.Retain);
    }
}
