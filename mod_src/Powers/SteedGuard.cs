using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Powers;

public sealed class SteedGuard : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPlayVfx => false;

    public override Creature ModifyUnblockedDamageTarget(Creature target, decimal _, ValueProp props, Creature? __)
    {
        if (target != base.Owner.PetOwner?.Creature)
        {
            return target;
        }
        if (!props.IsPoweredAttack())
        {
            return target;
        }
        // 白马存活：伤害转白马
        if (base.Owner.IsAlive)
        {
            return base.Owner;
        }
        // 白马已死：溢出伤害转黑马（灵幽马），黑马也死则原目标
        var spectrier = base.Owner.PetOwner?.PlayerCombatState?.GetPet<CalyrexMod.Monsters.Spectrier>();
        return (spectrier != null && spectrier.IsAlive) ? spectrier : target;
    }

    public override bool ShouldAllowHitting(Creature creature)
    {
        return creature.IsAlive;
    }

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        if (creature != base.Owner)
        {
            return true;
        }
        return false;
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }
}
