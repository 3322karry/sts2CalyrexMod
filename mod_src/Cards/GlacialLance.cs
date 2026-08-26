using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class GlacialLance : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DamageVar(4m, ValueProp.Move);
            yield return new IntVar("Hits", 2m);
            yield return new IntVar("Frost", 2m);
        }
    }

    public GlacialLance()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<ColorlessCardPool>();

    // 只有骑雪暴马（白马）时才能使用
    protected override bool IsPlayable => base.Owner != null && base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hits = base.DynamicVars["Hits"].IntValue;
        for (int i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        // 对所有敌人施加冰冻
        int frost = base.DynamicVars["Frost"].IntValue;
        foreach (var enemy in base.CombatState.Enemies.Where((Creature e) => e.IsAlive))
        {
            await PowerCmd.Apply<FrozenPower>(choiceContext, enemy, frost, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["Hits"].UpgradeValueBy(1m);
        base.DynamicVars["Frost"].UpgradeValueBy(1m);
    }
}
