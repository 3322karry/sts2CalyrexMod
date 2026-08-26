using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Relics;

// 特性膏药：捡起时为最多 6 张能力牌添加固有；
// 每场战斗第一回合你的前 2 张能力牌能够免费打出
public sealed class AbilityCapsule : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task AfterObtained()
    {
        // 为最多 6 张能力牌添加固有
        var deck = PileType.Deck.GetPile(base.Owner).Cards
            .Where((CardModel c) => c.Type == CardType.Power && !c.Keywords.Contains(CardKeyword.Innate))
            .Take(6)
            .ToList();
        foreach (var card in deck)
        {
            card.AddKeyword(CardKeyword.Innate);
        }
        // 挂免费能力 Power
        await PowerCmd.Apply<AbilityCapsulePower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 2m, base.Owner.Creature, null);
    }
}
