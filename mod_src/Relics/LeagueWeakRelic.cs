using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Relics;

// 联赛怯场标记（事件内部）：下场战斗开始时获得 2 虚弱，然后自毁
public sealed class LeagueWeakRelic : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task BeforeCombatStart()
    {
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 2m, base.Owner.Creature, null);
        await RelicCmd.Remove(this);
    }
}
