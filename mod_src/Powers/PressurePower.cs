using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Powers;

public sealed class PressurePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 敌人每次攻击时获得易伤
    public override async Task BeforeAttack(AttackCommand command)
    {
        var attacker = command.Attacker;
        if (attacker == null || attacker.IsPlayer || base.Owner == null)
        {
            return;
        }
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), attacker, System.Math.Max(1m, base.Amount), base.Owner, null);
    }
}
