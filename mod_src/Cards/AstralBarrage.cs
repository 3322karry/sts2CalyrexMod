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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class AstralBarrage : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DamageVar(0m, ValueProp.Move);
            yield return new IntVar("Hits", 2m);
            yield return new IntVar("HitDamage", 0m);
        }
    }

    public AstralBarrage()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<ColorlessCardPool>();

    protected override bool HasEnergyCostX => true;

    // 只有骑灵幽马（黑马）时才能使用
    protected override bool IsPlayable => base.Owner != null && base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedSpectrier);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int xValue = ResolveEnergyXValue();
        int hits = xValue + base.DynamicVars["Hits"].IntValue;
        int hitDamage = xValue + base.DynamicVars["HitDamage"].IntValue;

        await DamageCmd.Attack(hitDamage)
            .WithHitCount(hits)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 给所有敌人施加 1 层虚弱
        foreach (var enemy in base.CombatState.Enemies.Where((Creature e) => e.IsAlive))
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, 1m, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Hits"].UpgradeValueBy(1m);
        base.DynamicVars["HitDamage"].UpgradeValueBy(2m);
    }
}
