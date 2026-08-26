using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Relics;

public sealed class FocusSash : RelicModel
{
    private bool _used;

    public override RelicRarity Rarity => RelicRarity.Rare;

    // 受到致命伤害时，伤害削减到只剩 1 点血量；此遗物失效
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_used || target != base.Owner?.Creature || target.IsDead)
        {
            return 0m;
        }
        if (amount >= target.CurrentHp)
        {
            _used = true;
            return (target.CurrentHp - 1m) - amount;
        }
        return 0m;
    }
}
