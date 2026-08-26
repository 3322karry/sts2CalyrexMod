using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Relics;

public sealed class ExpShare : RelicModel
{
    private bool _usedThisCombat;

    public override RelicRarity Rarity => RelicRarity.Common;

    // 战斗中打出的第一张未升级的牌临时升级（打出前升级，OnPlay 生效）
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!_usedThisCombat && cardPlay.Card.Owner == base.Owner && !cardPlay.Card.IsUpgraded && cardPlay.Card.IsUpgradable)
        {
            _usedThisCombat = true;
            CardCmd.Upgrade(cardPlay.Card);
        }
        return Task.CompletedTask;
    }

    public override async Task BeforeCombatStart()
    {
        _usedThisCombat = false;
        await Task.CompletedTask;
    }
}
