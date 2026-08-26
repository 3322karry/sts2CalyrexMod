using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Cards;

public sealed class StrengthSap : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Percent", 30m);
            yield return new IntVar("StrengthLoss", 3m);
        }
    }

    public StrengthSap()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 敌人即将造成的伤害（意图）
        decimal incoming = 0m;
        var nextMove = cardPlay.Target.Monster?.NextMove;
        if (nextMove != null)
        {
            foreach (var intent in nextMove.Intents)
            {
                if (intent is MegaCrit.Sts2.Core.MonsterMoves.Intents.AttackIntent attack)
                {
                    incoming += attack.GetTotalDamage(new[] { base.Owner.Creature }, cardPlay.Target);
                }
            }
        }

        // 回复即将造成伤害的 X%
        int percent = base.DynamicVars["Percent"].IntValue;
        decimal heal = incoming * percent / 100m;
        if (heal > 0m)
        {
            await CreatureCmd.Heal(base.Owner.Creature, heal);
        }

        // 减少目标 3 力量
        await PowerCmd.Apply<StrengthPower>(choiceContext, cardPlay.Target, -base.DynamicVars["StrengthLoss"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Percent"].UpgradeValueBy(20m);
    }
}
