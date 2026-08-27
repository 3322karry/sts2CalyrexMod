using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

namespace CalyrexMod.Cards;

public sealed class MountChoiceSpectrier : CardModel
{
    public MountChoiceSpectrier()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.MountedSpectrierTip;
        }
    }


}
