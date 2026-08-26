using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace CalyrexMod.Monsters;

// 自定义意图：显示为 减（效果）/攻（X*Y）/增/牌/效/防/回/晕 文本。
// 文本 key 预定义在 monsters.json（如 INCINEROAR.intent.attack = "攻（15（16））"）。
public abstract class CustomIntent : AbstractIntent
{
    private readonly string _locKey;

    protected CustomIntent(string locKey)
    {
        _locKey = locKey;
    }

    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}

public sealed class AttackIntentCustom : CustomIntent
{
    public override IntentType IntentType => IntentType.Attack;
    protected override string IntentPrefix => "ATTACK";
    protected override string? SpritePath => "atlases/intent_atlas.sprites/intent_attack.tres";
    public AttackIntentCustom(string locKey) : base(locKey) { }
}

public sealed class DebuffIntentCustom : CustomIntent
{
    public override IntentType IntentType => IntentType.Debuff;
    protected override string IntentPrefix => "DEBUFF";
    protected override string? SpritePath => "atlases/intent_atlas.sprites/intent_debuff.tres";
    public DebuffIntentCustom(string locKey) : base(locKey) { }
}

public sealed class BuffIntentCustom : CustomIntent
{
    public override IntentType IntentType => IntentType.Buff;
    protected override string IntentPrefix => "BUFF";
    protected override string? SpritePath => "atlases/intent_atlas.sprites/intent_buff.tres";
    public BuffIntentCustom(string locKey) : base(locKey) { }
}

public sealed class DefendIntentCustom : CustomIntent
{
    public override IntentType IntentType => IntentType.Defend;
    protected override string IntentPrefix => "DEFEND";
    protected override string? SpritePath => "atlases/intent_atlas.sprites/intent_defend.tres";
    public DefendIntentCustom(string locKey) : base(locKey) { }
}

public sealed class HealIntentCustom : CustomIntent
{
    public override IntentType IntentType => IntentType.Heal;
    protected override string IntentPrefix => "HEAL";
    protected override string? SpritePath => "atlases/intent_atlas.sprites/intent_heal.tres";
    public HealIntentCustom(string locKey) : base(locKey) { }
}

public sealed class StatusIntentCustom : CustomIntent
{
    public override IntentType IntentType => IntentType.StatusCard;
    protected override string IntentPrefix => "STATUS";
    protected override string? SpritePath => "atlases/intent_atlas.sprites/intent_status.tres";
    public StatusIntentCustom(string locKey) : base(locKey) { }
}

public sealed class StunIntentCustom : CustomIntent
{
    public override IntentType IntentType => IntentType.Stun;
    protected override string IntentPrefix => "STUN";
    protected override string? SpritePath => "atlases/intent_atlas.sprites/intent_stun.tres";
    public StunIntentCustom(string locKey) : base(locKey) { }
}

public sealed class EffectIntentCustom : CustomIntent
{
    public override IntentType IntentType => IntentType.Buff;
    protected override string IntentPrefix => "BUFF";
    protected override string? SpritePath => "atlases/intent_atlas.sprites/intent_buff.tres";
    public EffectIntentCustom(string locKey) : base(locKey) { }
}
