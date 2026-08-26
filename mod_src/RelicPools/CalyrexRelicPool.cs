using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using CalyrexMod.Relics;

namespace CalyrexMod.RelicPools;

public sealed class CalyrexRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "ironclad";

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        yield return ModelDb.Relic<BlackWhiteCarrot>();
        yield return ModelDb.Relic<SereneGrace>();
        yield return ModelDb.Relic<FocusSash>();
        yield return ModelDb.Relic<NeverMeltIce>();
        yield return ModelDb.Relic<SpellTag>();
        yield return ModelDb.Relic<Eviolite>();
        yield return ModelDb.Relic<Multiscale>();
        yield return ModelDb.Relic<ExpShare>();
        yield return ModelDb.Relic<MiracleSeed>();
        yield return ModelDb.Relic<LoadedDice>();
        yield return ModelDb.Relic<Disguise>();
        yield return ModelDb.Relic<SnowCarrot>();
        yield return ModelDb.Relic<KingRock>();
        yield return ModelDb.Relic<AbilityCapsule>();
        yield return ModelDb.Relic<CalyrexOrobasTouch>();
    }
}
