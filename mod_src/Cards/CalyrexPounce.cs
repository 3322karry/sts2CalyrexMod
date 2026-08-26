using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Monsters;

namespace CalyrexMod.Cards;

public sealed class CalyrexPounce : CardModel
{
    public CalyrexPounce()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal damage = base.Owner.Creature.Block;
        if (base.IsUpgraded)
        {
            var combatState = base.Owner.PlayerCombatState;
            if (combatState != null)
            {
                var g = combatState.GetPet<Glastrier>();
                if (g != null && g.IsAlive) damage += g.MaxHp;
                var s = combatState.GetPet<Spectrier>();
                if (s != null && s.IsAlive) damage += s.MaxHp;
            }
        }
        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
