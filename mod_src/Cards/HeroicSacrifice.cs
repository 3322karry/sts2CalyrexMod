using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Monsters;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class HeroicSacrifice : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Per2", 1m);
        }
    }

    public HeroicSacrifice()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<Abundance>();
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = base.Owner.PlayerCombatState;
        if (combatState == null || base.Owner.Creature.CombatState == null)
        {
            return;
        }

        // 选择一匹活马牺牲
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

        int hpLost = steed.CurrentHp;
        await PowerCmd.Remove<SteedGuard>(steed);
        await CreatureCmd.Kill(steed, force: true);

        // 每损失 10 血量获得 X 丰饶
        int per2 = base.DynamicVars["Per2"].IntValue;
        int abundance = hpLost / 2 * per2;
        if (abundance > 0)
        {
            await PowerCmd.Apply<Abundance>(choiceContext, base.Owner.Creature, abundance, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后只把费用降为 0
        MockSetEnergyCost(new CardEnergyCost(this, 0, costsX: false));
    }
}
