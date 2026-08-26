using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Cards;

public sealed class MountChoiceGlastrier : CardModel
{
    public MountChoiceGlastrier()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();
}
