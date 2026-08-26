using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace CalyrexMod.Monsters;

// 自定义意图：显示为 减（效果）/攻（X*Y）/增/牌/效/防/回/晕 文本。
// 继承官方意图类（游戏会对 AttackIntent 等强转），仅覆盖显示文本（key 在 monsters.json）。
public sealed class AttackIntentCustom : SingleAttackIntent
{
    private readonly string _locKey;
    public AttackIntentCustom(int damage, string locKey) : base(damage)
    {
        _locKey = locKey;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}

public sealed class DebuffIntentCustom : DebuffIntent
{
    private readonly string _locKey;
    public DebuffIntentCustom(string locKey) : base(false)
    {
        _locKey = locKey;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}

public sealed class BuffIntentCustom : BuffIntent
{
    private readonly string _locKey;
    public BuffIntentCustom(string locKey)
    {
        _locKey = locKey;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}

public sealed class DefendIntentCustom : DefendIntent
{
    private readonly string _locKey;
    public DefendIntentCustom(string locKey)
    {
        _locKey = locKey;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}

public sealed class HealIntentCustom : HealIntent
{
    private readonly string _locKey;
    public HealIntentCustom(string locKey)
    {
        _locKey = locKey;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}

public sealed class StatusIntentCustom : StatusIntent
{
    private readonly string _locKey;
    public StatusIntentCustom(string locKey) : base(1)
    {
        _locKey = locKey;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}

public sealed class StunIntentCustom : StunIntent
{
    private readonly string _locKey;
    public StunIntentCustom(string locKey)
    {
        _locKey = locKey;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}

public sealed class EffectIntentCustom : BuffIntent
{
    private readonly string _locKey;
    public EffectIntentCustom(string locKey)
    {
        _locKey = locKey;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return new LocString("monsters", _locKey);
    }
}
