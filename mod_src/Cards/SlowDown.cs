using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class SlowDown : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Frost", 5m);
        }
    }

    public SlowDown()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<SlowDownPower>();
            yield return HoverTipFactory.FromPower<FrozenPower>();
        }
    }

    // 只有骑雪暴马（白马）时才能使用
    protected override bool IsPlayable => base.Owner != null && base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 给所有敌人 5 层冰冻
        int frost = base.DynamicVars["Frost"].IntValue;
        foreach (var enemy in base.CombatState.Enemies.Where((Creature e) => e.IsAlive))
        {
            await PowerCmd.Apply<FrozenPower>(choiceContext, enemy, frost, base.Owner.Creature, this);
        }

        // 每使用 2 费，复制上一张打出的牌（0 费消耗）入手
        await PowerCmd.Apply<SlowDownPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Frost"].UpgradeValueBy(3m);
    }
}
