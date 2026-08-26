using System.Linq;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Relics;

public sealed class LoadedDice : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    // 骑马时，使用费用为 X 的牌时 X+2
    public override int ModifyXValue(CardModel card, int originalValue)
    {
        if (base.Owner == null || card.Owner != base.Owner)
        {
            return originalValue;
        }
        bool mounted = base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedGlastrier || p is MountedSpectrier);
        return mounted ? originalValue + 2 : originalValue;
    }
}
