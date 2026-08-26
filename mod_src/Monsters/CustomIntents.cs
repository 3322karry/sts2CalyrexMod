using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace CalyrexMod.Monsters;

// 自定义意图：显示为 减（效果）/攻（X*Y）/增/牌/效/防/回/晕 文本。
// 显示当前数值（无进阶括号）：传基 key，进阶时自动用 {key}_A。
// 继承官方意图类（游戏会对 AttackIntent 等强转）。
public static class IntentLabelHelper
{
    public static LocString Get(string baseKey, bool ascension)
    {
        return new LocString("monsters", ascension ? baseKey + "_A" : baseKey);
    }
}

public sealed class AttackIntentCustom : SingleAttackIntent
{
    private readonly string _baseKey;
    private readonly bool _asc;
    public AttackIntentCustom(int damage, string baseKey, bool ascension) : base(damage)
    {
        _baseKey = baseKey;
        _asc = ascension;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentLabelHelper.Get(_baseKey, _asc);
    }
}

public sealed class DebuffIntentCustom : DebuffIntent
{
    private readonly string _baseKey;
    private readonly bool _asc;
    public DebuffIntentCustom(string baseKey, bool ascension) : base(false)
    {
        _baseKey = baseKey;
        _asc = ascension;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentLabelHelper.Get(_baseKey, _asc);
    }
}

public sealed class BuffIntentCustom : BuffIntent
{
    private readonly string _baseKey;
    private readonly bool _asc;
    public BuffIntentCustom(string baseKey, bool ascension)
    {
        _baseKey = baseKey;
        _asc = ascension;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentLabelHelper.Get(_baseKey, _asc);
    }
}

public sealed class DefendIntentCustom : DefendIntent
{
    private readonly string _baseKey;
    private readonly bool _asc;
    public DefendIntentCustom(string baseKey, bool ascension)
    {
        _baseKey = baseKey;
        _asc = ascension;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentLabelHelper.Get(_baseKey, _asc);
    }
}

public sealed class HealIntentCustom : HealIntent
{
    private readonly string _baseKey;
    private readonly bool _asc;
    public HealIntentCustom(string baseKey, bool ascension)
    {
        _baseKey = baseKey;
        _asc = ascension;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentLabelHelper.Get(_baseKey, _asc);
    }
}

public sealed class StatusIntentCustom : StatusIntent
{
    private readonly string _baseKey;
    private readonly bool _asc;
    public StatusIntentCustom(string baseKey, bool ascension) : base(1)
    {
        _baseKey = baseKey;
        _asc = ascension;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentLabelHelper.Get(_baseKey, _asc);
    }
}

public sealed class StunIntentCustom : StunIntent
{
    private readonly string _baseKey;
    private readonly bool _asc;
    public StunIntentCustom(string baseKey, bool ascension)
    {
        _baseKey = baseKey;
        _asc = ascension;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentLabelHelper.Get(_baseKey, _asc);
    }
}

public sealed class EffectIntentCustom : BuffIntent
{
    private readonly string _baseKey;
    private readonly bool _asc;
    public EffectIntentCustom(string baseKey, bool ascension)
    {
        _baseKey = baseKey;
        _asc = ascension;
    }
    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        return IntentLabelHelper.Get(_baseKey, _asc);
    }
}
