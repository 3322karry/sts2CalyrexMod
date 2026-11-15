using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Monsters;

namespace CalyrexMod.Cards;

public sealed class CalyrexPounce : CardModel
{
    public CalyrexPounce()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    // 伤害 = 当前格挡（升级后再加上两只马的最大生命）。
    // 用 CalculatedDamageVar 而非手写数字，手里/出牌预览会走 Hook.ModifyDamage，显示最终伤害。
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(0m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
        {
            decimal damage = card.Owner.Creature.Block;
            if (card.IsUpgraded)
            {
                var combatState = card.Owner.PlayerCombatState;
                if (combatState != null)
                {
                    var g = combatState.GetPet<Glastrier>();
                    if (g != null && g.IsAlive) damage += g.MaxHp;
                    var s = combatState.GetPet<Spectrier>();
                    if (s != null && s.IsAlive) damage += s.MaxHp;
                }
            }
            return damage;
        })
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
