using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Relics;

public sealed class Multiscale : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // 满血时受到伤害减 50%
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner?.Creature || !props.IsPoweredAttack())
        {
            return 1m;
        }
        if (target.CurrentHp >= target.MaxHp)
        {
            return 0.5m;
        }
        return 1m;
    }
}
