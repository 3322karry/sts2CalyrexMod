using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Relics;

public sealed class SereneGrace : RelicModel
{
    private int _usesLeft = 3;

    public override RelicRarity Rarity => RelicRarity.Rare;

    // 每次战斗前三次给予的异常效果（debuff）翻倍
    public override decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (giver == base.Owner?.Creature && power.Type == PowerType.Debuff && _usesLeft > 0 && amount > 0m)
        {
            _usesLeft--;
            return 2m;
        }
        return 1m;
    }

    // 每场战斗重置使用次数
    public override async Task BeforeCombatStart()
    {
        _usesLeft = 3;
        await Task.CompletedTask;
    }
}
