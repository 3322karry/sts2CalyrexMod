using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Cards;

public sealed class PollenPuff : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DamageVar(9m, ValueProp.Move);
            yield return new IntVar("HealPct", 33m);
        }
    }

    public PollenPuff()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal healPct = base.DynamicVars["HealPct"].IntValue / 100m;
        if (cardPlay.Target.PetOwner != null)
        {
            // 目标就是马：回复其 33%（升级 50%）最大生命
            await CreatureCmd.Heal(cardPlay.Target, cardPlay.Target.MaxHp * healPct, playAnim: true);
            return;
        }
        // 敌人：造成 9（升级 12）点伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // 花粉飘散：两匹马也回复 33%（升级 50%）最大生命
        if (base.Owner.PlayerCombatState != null)
        {
            var glastrier = base.Owner.PlayerCombatState.GetPet<CalyrexMod.Monsters.Glastrier>();
            if (glastrier != null && glastrier.IsAlive)
            {
                await CreatureCmd.Heal(glastrier, glastrier.MaxHp * healPct, playAnim: true);
            }
            var spectrier = base.Owner.PlayerCombatState.GetPet<CalyrexMod.Monsters.Spectrier>();
            if (spectrier != null && spectrier.IsAlive)
            {
                await CreatureCmd.Heal(spectrier, spectrier.MaxHp * healPct, playAnim: true);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["HealPct"].UpgradeValueBy(17m);
    }
}
