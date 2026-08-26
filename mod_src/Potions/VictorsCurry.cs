using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CalyrexMod.Potions;

public sealed class VictorsCurry : PotionModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new IntVar("Heal", 5m);
            yield return new IntVar("Cards", 3m);
            yield return new IntVar("Regen", 3m);
        }
    }

    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<RegenPower>();
        }
    }

    // 回复 5 血，抽 3 张牌，获得 3 再生
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);
        await CreatureCmd.Heal(target, base.DynamicVars["Heal"].BaseValue, playAnim: false);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars["Cards"].BaseValue, base.Owner);
        await PowerCmd.Apply<RegenPower>(choiceContext, target, base.DynamicVars["Regen"].BaseValue, target, null);
    }
}
