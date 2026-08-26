using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using CalyrexMod.Cards;

namespace CalyrexMod.CardPools;

public sealed class CalyrexCardPool : CardPoolModel
{
    public override string Title => "calyrex";

    public override string EnergyColorName => "calyrex";

    public override string CardFrameMaterialPath => "card_frame_calyrex";

    public override Color DeckEntryCardColor => new Color("007A79");

    public override Color EnergyOutlineColor => new Color("007A79");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return Array.Empty<CardModel>();
    }
}
