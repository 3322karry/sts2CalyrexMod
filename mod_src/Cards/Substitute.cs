using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Cards;

public sealed class Substitute : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("HpLoss", 6m);
            yield return new IntVar("Buffer", 1m);
        }
    }

    public Substitute()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 受到 6 点伤害（不可格挡）
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, this);

        // 获得缓冲
        await PowerCmd.Apply<BufferPower>(choiceContext, base.Owner.Creature, base.DynamicVars["Buffer"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HpLoss"].UpgradeValueBy(2m);
        base.DynamicVars["Buffer"].UpgradeValueBy(1m);
    }
}
