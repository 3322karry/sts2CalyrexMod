using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Relics;

public sealed class Disguise : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    // 战斗开始时获得 1 层缓冲
    public override async Task BeforeCombatStart()
    {
        if (base.Owner != null)
        {
            await PowerCmd.Apply<BufferPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 1m, base.Owner.Creature, null);
        }
    }
}
