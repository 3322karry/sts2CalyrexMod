using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class AbsoluteZero : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Threshold", 15m);
        }
    }

    public AbsoluteZero()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.FrozenTip;
        }
    }

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 冰冻层数 ≥ 12 的敌人死亡
        if (base.CombatState == null)
        {
            return;
        }
        int threshold = base.DynamicVars["Threshold"].IntValue;
        var doomed = base.CombatState.Enemies
            .Where((Creature e) => e.IsAlive && e.Powers.FirstOrDefault((PowerModel p) => p is FrozenPower)?.Amount >= threshold)
            .ToList();
        foreach (var enemy in doomed)
        {
            await CreatureCmd.Kill(enemy, force: true);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
