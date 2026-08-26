using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

// 精神噪音：使目标接下来 X 次回复无效
public sealed class PsychicNoisePower : PowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != base.Owner || delta <= 0m)
        {
            return;
        }
        // 抵消回复
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, delta, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PowerCmd.TickDownDuration(this);
    }
}
