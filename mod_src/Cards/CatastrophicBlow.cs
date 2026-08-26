using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Cards;

public sealed class CatastrophicBlow : CardModel
{
    public CatastrophicBlow()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 令敌人失去一半血量；Boss 固定 30 点
        decimal damage;
        if (cardPlay.Target.CombatState?.Encounter?.RoomType == MegaCrit.Sts2.Core.Rooms.RoomType.Boss)
        {
            damage = 30m;
        }
        else
        {
            damage = cardPlay.Target.MaxHp / 2m;
        }

        await CreatureCmd.Damage(choiceContext, cardPlay.Target, damage, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        MockSetEnergyCost(new CardEnergyCost(this, 1, costsX: false));
    }
}
