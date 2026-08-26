using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using CalyrexMod.Cards;

namespace CalyrexMod.Powers;

public sealed class HelpingHandPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<HelpingHand>();
}
