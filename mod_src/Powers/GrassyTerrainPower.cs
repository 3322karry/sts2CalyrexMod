using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Monsters;

namespace CalyrexMod.Powers;

public sealed class GrassyTerrainPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 回合开始时：喂养 3 + 丰饶 2
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || base.Owner?.Player?.PlayerCombatState == null)
        {
            return;
        }

        var player = base.Owner.Player!;
        var pcs = player.PlayerCombatState!;
        Creature? glastrier = pcs.GetPet<Glastrier>();
        if (glastrier != null)
        {
            await CreatureCmd.GainMaxHp(glastrier, 3m * base.Amount);
        }
        else
        {
            // 白马不在场：重新召唤再喂养
            glastrier = await CalyrexMod.Cards.MountHelper.SpawnSteed(player, typeof(CalyrexMod.Monsters.Glastrier));
            await CreatureCmd.GainMaxHp(glastrier, 3m * base.Amount);
        }
        Creature? spectrier = pcs.GetPet<Spectrier>();
        if (spectrier != null)
        {
            await CreatureCmd.GainMaxHp(spectrier, 3m * base.Amount);
        }
        else
        {
            spectrier = await CalyrexMod.Cards.MountHelper.SpawnSteed(player, typeof(CalyrexMod.Monsters.Spectrier));
            await CreatureCmd.GainMaxHp(spectrier, 3m * base.Amount);
        }

        await PowerCmd.Apply<Abundance>(new ThrowingPlayerChoiceContext(), base.Owner, 2m * base.Amount, base.Owner, null);
    }
}
