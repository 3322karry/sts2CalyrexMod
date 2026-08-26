using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using CalyrexMod.Potions;

namespace CalyrexMod.PotionPools;

public sealed class CalyrexPotionPool : PotionPoolModel
{
    public override string EnergyColorName => "ironclad";

    protected override IEnumerable<PotionModel> GenerateAllPotions()
    {
        yield return ModelDb.Potion<FigyBerry>();
        yield return ModelDb.Potion<GrayCarrot>();
        yield return ModelDb.Potion<DefenseBoost>();
        yield return ModelDb.Potion<GalarianSpice>();
        yield return ModelDb.Potion<VictorsCurry>();
    }
}
