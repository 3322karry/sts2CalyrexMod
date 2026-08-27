using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Cards;

public sealed class Protect : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new BlockVar(999m, ValueProp.Unpowered);
        }
    }

    public Protect()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    // 回合内没有使用过牌时才能使用（通过遗物计数判定）
    protected override bool IsPlayable => base.Owner == null
        || base.Owner.GetRelic<CalyrexMod.Relics.BlackWhiteCarrot>() is not CalyrexMod.Relics.BlackWhiteCarrot carrot
        || !carrot.HasPlayedCardThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue, ValueProp.Unpowered, cardPlay, fast: true);
        PlayerCmd.EndTurn(base.Owner, canBackOut: false);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
        AddKeyword(CardKeyword.Retain);
    }
}
