using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Monsters;
using CalyrexMod.Powers;

namespace CalyrexMod.Relics;

public sealed class SnowCarrot : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool AddsPet => true;

    public override bool SpawnsPets => true;

    // 战斗开始：喂养 20，为黑/白马各 +1 标记
    public override async Task BeforeCombatStart()
    {
        if (base.Owner == null || base.Owner.PlayerCombatState == null)
        {
            return;
        }

        Creature glastrier = await CalyrexMod.Cards.MountHelper.SpawnSteed(base.Owner, typeof(CalyrexMod.Monsters.Glastrier));
        Creature spectrier = await CalyrexMod.Cards.MountHelper.SpawnSteed(base.Owner, typeof(CalyrexMod.Monsters.Spectrier));

        // 喂养 20
        await CreatureCmd.GainMaxHp(glastrier, 20m);
        await CreatureCmd.GainMaxHp(spectrier, 20m);

        // 双马挡伤 + 洗入机制（挂蕾冠王身上）
        await PowerCmd.Apply<SteedGuard>(new ThrowingPlayerChoiceContext(), glastrier, 1m, base.Owner.Creature, null, silent: true);
        await PowerCmd.Apply<SteedGuard>(new ThrowingPlayerChoiceContext(), spectrier, 1m, base.Owner.Creature, null, silent: true);
        if (!base.Owner.Creature.Powers.Any((PowerModel p) => p is HeavyLance))
        {
            await PowerCmd.Apply<HeavyLance>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 1m, base.Owner.Creature, null, silent: true);
        }
        if (!base.Owner.Creature.Powers.Any((PowerModel p) => p is QuickSight))
        {
            await PowerCmd.Apply<QuickSight>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 1m, base.Owner.Creature, null, silent: true);
        }

    }
}
