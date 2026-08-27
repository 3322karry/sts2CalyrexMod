using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Monsters;

namespace CalyrexMod.Cards;

public sealed class Boomburst : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new DamageVar(18m, ValueProp.Move);
            yield return new IntVar("SelfReduce", 25m);
        }
    }

    public Boomburst()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人造成全额伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 对两匹马也造成全额伤害（爆音波震全场）
        if (base.Owner.PlayerCombatState != null)
        {
            var glastrier = base.Owner.PlayerCombatState.GetPet<CalyrexMod.Monsters.Glastrier>();
            if (glastrier != null && glastrier.IsAlive)
            {
                await CreatureCmd.Damage(choiceContext, glastrier, base.DynamicVars.Damage.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner.Creature, this);
            }
            var spectrier = base.Owner.PlayerCombatState.GetPet<CalyrexMod.Monsters.Spectrier>();
            if (spectrier != null && spectrier.IsAlive)
            {
                await CreatureCmd.Damage(choiceContext, spectrier, base.DynamicVars.Damage.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner.Creature, this);
            }
        }

        // 我方（自己）承受削减后的伤害（升级后固定失去 7 血）
        int reduce = base.DynamicVars["SelfReduce"].IntValue;
        decimal selfDamage = base.DynamicVars.Damage.BaseValue * (100 - reduce) / 100m;
        if (base.IsUpgraded)
        {
            selfDamage = 7m;
        }
        if (selfDamage > 0m)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, selfDamage, ValueProp.Unblockable | ValueProp.Unpowered, null, this);
        }
    }

        protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return KeywordTipHelper.FeedTip;
        }
    }

protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        // 升级后自身固定失去 7 血
        base.DynamicVars["SelfReduce"].BaseValue = 0m;
    }
}
