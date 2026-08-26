using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Cards;

namespace CalyrexMod.Potions;

public sealed class GrayCarrot : PotionModel
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    // 骑马
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await MountHelper.DoMount(choiceContext, base.Owner, null);
    }
}
