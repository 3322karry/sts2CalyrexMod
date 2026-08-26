using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Cards;

public sealed class TeraBlast : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DamageVar(12m, ValueProp.Move);
            yield return new IntVar("Debuff", 3m);
        }
    }

    public TeraBlast()
        : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 随机施加虚弱/易伤/灾厄/中毒
        int debuff = base.DynamicVars["Debuff"].IntValue;
        int roll = base.Owner.PlayerRng.Rewards.NextInt(0, 4);
        switch (roll)
        {
            case 0:
                await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, debuff, base.Owner.Creature, this);
                break;
            case 1:
                await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, debuff, base.Owner.Creature, this);
                break;
            case 2:
                await PowerCmd.Apply<DoomPower>(choiceContext, cardPlay.Target, debuff, base.Owner.Creature, this);
                break;
            default:
                await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, debuff, base.Owner.Creature, this);
                break;
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Debuff"].UpgradeValueBy(2m);
    }
}
