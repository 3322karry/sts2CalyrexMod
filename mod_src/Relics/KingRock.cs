using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Relics;

// 王者之证：回合内每次攻击有 8% 概率击晕目标
public sealed class KingRock : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature? target, CardModel? cardSource)
    {
        if (dealer != base.Owner?.Creature || target == null || target.PetOwner != null)
        {
            return;
        }
        if (base.Owner?.PlayerRng != null && base.Owner.PlayerRng.Rewards.NextInt(0, 100) < 8)
        {
            await CreatureCmd.Stun(target, (_) => Task.CompletedTask);
        }
    }
}
