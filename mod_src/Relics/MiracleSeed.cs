using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Relics;

public sealed class MiracleSeed : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    // 战斗开始时获得 2 层丰饶
    public override async Task BeforeCombatStart()
    {
        if (base.Owner != null)
        {
            await PowerCmd.Apply<Abundance>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 2m, base.Owner.Creature, null);
        }
    }
}
