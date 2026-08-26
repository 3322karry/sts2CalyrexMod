using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Cards;
using CalyrexMod.Powers;

namespace CalyrexMod.Relics;

public sealed class NeverMeltIce : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    // 骑白马时，升级所有雪矛
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || base.Owner == null || !base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier))
        {
            return;
        }
        var piles = new[] { PileType.Draw, PileType.Hand, PileType.Discard, PileType.Exhaust };
        foreach (var pileType in piles)
        {
            var pile = pileType.GetPile(base.Owner);
            foreach (var card in pile.Cards.Where((CardModel c) => c is GlacialLance && !c.IsUpgraded).ToList())
            {
                CardCmd.Upgrade(card);
            }
        }
        await Task.CompletedTask;
    }
}
