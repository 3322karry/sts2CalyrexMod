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
            yield return new DamageVar(0m, ValueProp.Move);
            yield return new IntVar("Multiplier", 5m);
        }
    }

    public GlacialLance()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<ColorlessCardPool>();

    protected override bool HasEnergyCostX => true;

    // 只有骑雪暴马（白马）时才能使用
    protected override bool IsPlayable => base.Owner != null && base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int xValue = ResolveEnergyXValue();
        int damage = xValue * base.DynamicVars["Multiplier"].IntValue;

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 升级后：为所有敌人增加 1 层冰冻
        if (base.IsUpgraded)
        {
            foreach (var enemy in base.CombatState.Enemies.Where((Creature e) => e.IsAlive))
            {
                await PowerCmd.Apply<FrozenPower>(choiceContext, enemy, 1m, base.Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Multiplier"].UpgradeValueBy(3m);
    }
}
