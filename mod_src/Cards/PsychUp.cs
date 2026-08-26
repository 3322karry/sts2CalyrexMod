using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Cards;

public sealed class PsychUp : CardModel
{
    public PsychUp()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 复制一个对象的正面效果（从任意存活单位随机选一个正面 Buff 复制到自身）
        var targets = base.CombatState?.Creatures.Where((Creature c) => c.IsAlive && c != base.Owner.Creature).ToList();
        if (targets == null || targets.Count == 0)
        {
            return;
        }
        var rng = base.Owner.PlayerRng.Rewards;
        var source = targets[rng.NextInt(targets.Count)];
        var buffs = source.Powers.Where((PowerModel p) => p.Type == PowerType.Buff && p.Amount > 0).ToList();
        if (buffs.Count == 0)
        {
            return;
        }
        var buff = buffs[rng.NextInt(buffs.Count)];
        await PowerCmd.Apply(choiceContext, buff, base.Owner.Creature, buff.Amount, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        MockSetEnergyCost(new CardEnergyCost(this, 0, costsX: false));
    }
}
