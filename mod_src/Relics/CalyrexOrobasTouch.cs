using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;
using CalyrexMod.Cards;
using CalyrexMod.Relics;

namespace CalyrexMod.Relics;

// 欧罗巴斯之触（蕾冠王版）：升级初始遗物（黑白萝卜→灵雪萝卜）+ 升级初始牌（牵绊缰绳→勇气绳索）
public sealed class CalyrexOrobasTouch : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public bool SetupForPlayer(Player player)
    {
        if (player == null)
        {
            return false;
        }
        bool hasCarrot = player.Relics.Any((RelicModel r) => r is BlackWhiteCarrot);
        bool hasReins = PileType.Deck.GetPile(player).Cards.Any((CardModel c) => c is BondedReins);
        return hasCarrot && hasReins;
    }

    public override async Task AfterObtained()
    {
        if (base.Owner == null)
        {
            return;
        }

        // 1. 黑白萝卜 → 灵雪萝卜（SnowCarrot）
        var carrot = base.Owner.Relics.FirstOrDefault((RelicModel r) => r is BlackWhiteCarrot);
        if (carrot != null)
        {
            await RelicCmd.Replace(carrot, ModelDb.Relic<SnowCarrot>().ToMutable());
        }

        // 2. 牵绊缰绳 → 勇气绳索（牌组中变化）
        var deck = PileType.Deck.GetPile(base.Owner).Cards
            .Where((CardModel c) => c is BondedReins)
            .ToList();
        foreach (var reins in deck)
        {
            var rope = base.Owner.RunState.CreateCard<CourageRope>(base.Owner);
            await CardCmd.Transform(reins, rope);
        }
    }
}
