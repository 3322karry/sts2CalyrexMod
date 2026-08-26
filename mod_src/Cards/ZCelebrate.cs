using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Monsters;

namespace CalyrexMod.Cards;

public sealed class ZCelebrate : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("FeedX", 5m);
            yield return new IntVar("StrDex", 1m);
            yield return new IntVar("BlockX", 5m);
        }
    }

    public ZCelebrate()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int x = ResolveEnergyXValue();
        var combatState = base.Owner.PlayerCombatState;
        if (combatState != null && x > 0)
        {
            decimal feed = base.DynamicVars["FeedX"].IntValue * x;
            var g = combatState.GetPet<Glastrier>();
            if (g != null) await CreatureCmd.GainMaxHp(g, feed);
            var s = combatState.GetPet<Spectrier>();
            if (s != null) await CreatureCmd.GainMaxHp(s, feed);
        }
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars["StrDex"].IntValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, base.DynamicVars["StrDex"].IntValue, base.Owner.Creature, this);
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars["BlockX"].IntValue * x, ValueProp.Unpowered, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["StrDex"].UpgradeValueBy(1m);
        base.DynamicVars["BlockX"].UpgradeValueBy(2m);
    }
}
