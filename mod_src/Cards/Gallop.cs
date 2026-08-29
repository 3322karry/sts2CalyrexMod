using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Monsters;

namespace CalyrexMod.Cards;

public sealed class Gallop : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("HpLoss", 4m);
        }
    }

    public Gallop()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.Owner.PlayerCombatState;
        if (combatState == null || base.Owner.Creature.CombatState == null)
        {
            return;
        }

        // 选择一匹马（目标指定）
        var choices = new List<CardModel>();
        Creature? liveGlastrier = combatState.GetPet<Glastrier>();
        if (liveGlastrier != null && liveGlastrier.IsAlive)
        {
            choices.Add(base.Owner.Creature.CombatState.CreateCard<MountChoiceGlastrier>(base.Owner));
        }
        Creature? liveSpectrier = combatState.GetPet<Spectrier>();
        if (liveSpectrier != null && liveSpectrier.IsAlive)
        {
            choices.Add(base.Owner.Creature.CombatState.CreateCard<MountChoiceSpectrier>(base.Owner));
        }
        if (choices.Count == 0)
        {
            return;
        }

        CardModel? chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, choices, base.Owner, canSkip: true);
        if (chosen == null)
        {
            return;
        }

        Creature? steed = chosen switch
        {
            MountChoiceGlastrier => combatState.GetPet<Glastrier>(),
            MountChoiceSpectrier => combatState.GetPet<Spectrier>(),
            _ => null
        };
        if (steed == null || !steed.IsAlive)
        {
            return;
        }

        // 选中的马失去血量（不可格挡）
        await CreatureCmd.Damage(choiceContext, steed, base.DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner.Creature, this);
        if (!steed.IsAlive)
        {
            return;
        }

        // 然后骑马合体
        await MountHelper.DoMount(choiceContext, base.Owner, this, steed);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HpLoss"].UpgradeValueBy(-1m);
    }
}
