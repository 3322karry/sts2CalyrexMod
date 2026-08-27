using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Monsters;

// 无极汰那 Eternatus（荣耀 Boss，两阶段，仿实验体多形态）：
// 阶段1：无极巨化；死亡后下回合复活进阶段2（消除自身效果）；血量<170 眩晕一回合
// 阶段2：混乱（破盾给虚弱/易伤/脆弱）；震慑（本回合玩家只能打一张攻击牌）
public sealed class Eternatus : MonsterModel
{
    private bool _isPhase2;
    private bool _revivePending;
    private bool _stunPending;
    private MoveState? _deadState;

    private const int StunThreshold = 170;

    public int FirstFormHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 408, 368);
    public int SecondFormHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 828, 768);

    public override int MinInitialHp => _isPhase2 ? SecondFormHp : FirstFormHp;
    public override int MaxInitialHp => MinInitialHp;

    protected override string VisualsPath => _isPhase2
        ? "res://CalyrexMod/monsters/eternatus_eternamax.tscn"
        : "res://CalyrexMod/monsters/eternatus.tscn";

    public bool IsPhase2 => _isPhase2;

    private bool Asc => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 1, 0) > 0;

    // 死亡：触发阶段转换（下回合复活）
    public async Task TriggerDeadState()
    {
        _revivePending = true;
        if (_deadState != null)
        {
            SetMoveImmediate(_deadState, forceTransition: true);
        }
        MegaCrit.Sts2.Core.Logging.Log.Info("[CalyrexMod] Eternatus died (phase1), reviving next turn");
    }

    // 战斗开始挂复活 Power（阻止战斗结束 + 死亡触发阶段转换）
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<EternatusRevivePower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        await PowerCmd.Apply<EternamaxPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        CalyrexMod.Audio.CalyrexBgmManager.PlayPhase1();
    }

    private async Task RespawnMove(IReadOnlyList<Creature> targets)
    {
        _revivePending = false;
        _isPhase2 = true;
        CalyrexMod.Audio.CalyrexBgmManager.PlayPhase2();
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Eternatus phase2 reviving, target hp={MinInitialHp}");
        await Cmd.Wait(0.8f);
        if (base.Creature.CombatState == null)
        {
            return;
        }
        // 消除自身所有效果（保留复活 Power，最后移除）
        foreach (var p in base.Creature.Powers.ToList())
        {
            if (p is EternatusRevivePower || p is EternamaxPower)
            {
                continue;
            }
            await PowerCmd.Remove(p);
        }
        // 先解除复活状态，再复活（SetMaxHp + Heal 官方复活法）
        base.Creature.GetPower<EternatusRevivePower>()?.DoRevive();
        decimal scaled = MegaCrit.Sts2.Core.Entities.Creatures.Creature.ScaleHpForMultiplayer(MinInitialHp, base.CombatState.Encounter, base.CombatState.Players.Count, base.CombatState.RunState.CurrentActIndex);
        await CreatureCmd.SetMaxHp(base.Creature, scaled);
        await CreatureCmd.Heal(base.Creature, scaled);
        // 阶段2 buff
        await PowerCmd.Apply<PanicPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        await PowerCmd.Apply<EternamaxPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
        // 阶段2 是最终阶段：移除复活 Power，此后死亡正常结束战斗（不会"阶段3"）
        await PowerCmd.Remove<EternatusRevivePower>(base.Creature);
        // 刷新视觉（无极巨化 + 放大 3 倍）
        CalyrexMod.Patching.EternatusVisualPatches.ApplyEternamaxVisual(base.Creature);
        MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Eternatus phase2 revived! hp={base.Creature.CurrentHp}/{base.Creature.MaxHp}");
    }

    // 血量 < 170 后眩晕一回合（阶段1）
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (_isPhase2 || creature != base.Creature || creature.IsDead)
        {
            return;
        }
        if (!_stunPending && creature.CurrentHp < StunThreshold)
        {
            _stunPending = true;
            await CreatureCmd.Stun(creature, (_) => Task.CompletedTask);
            MegaCrit.Sts2.Core.Logging.Log.Info("[CalyrexMod] Eternatus stunned below 170 HP");
        }
        await Task.CompletedTask;
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        if (_isPhase2)
        {
            return GeneratePhase2();
        }
        return GeneratePhase1();
    }

    private MonsterMoveStateMachine GeneratePhase1()
    {
        _deadState = new MoveState("RESPAWN_MOVE", RespawnMove, new HealIntentCustom("ETERNATUS.intent.respawn", false))
        {
            MustPerformOnceBeforeTransitioning = true
        };
        var status1 = new MoveState("STATUS1_MOVE", Status1Move, new StatusIntentCustom("ETERNATUS.intent.status1", Asc));
        var buff = new MoveState("BUFF_MOVE", BuffMove, new BuffIntentCustom("ETERNATUS.intent.buff", Asc));
        var bigHit = new MoveState("BIG_HIT_MOVE", BigHitMove, new AttackIntentCustom(BigHitDmg, "ETERNATUS.intent.bigHit", Asc));
        var multiHit = new MoveState("MULTI_HIT_MOVE", MultiHitMove, new AttackIntentCustom(MultiHitDmg, "ETERNATUS.intent.multiHit", Asc));
        var defend = new MoveState("DEFEND_MOVE", DefendMove, new DefendIntentCustom("ETERNATUS.intent.defend", Asc));

        status1.FollowUpState = bigHit;
        bigHit.FollowUpState = buff;
        buff.FollowUpState = multiHit;
        multiHit.FollowUpState = defend;
        defend.FollowUpState = bigHit;

        // 死亡后 SetMoveImmediate(DeadState)，下回合执行 RespawnMove 复活
        // 复活后经分支回到正常状态机（阶段2 用 phase2 状态；阶段1 用 status1）
        var p2States = GeneratePhase2StartState();
        var reviveBranch = new ConditionalBranchState("REVIVE_BRANCH");
        _deadState.FollowUpState = reviveBranch;
        reviveBranch.AddState(status1, () => !_isPhase2);
        reviveBranch.AddState(p2States[0], () => _isPhase2);
        var all = new List<MonsterState> { _deadState, reviveBranch, status1, buff, bigHit, multiHit, defend };
        all.AddRange(p2States);
        return new MonsterMoveStateMachine(all, status1);
    }

    // 阶段2 全部状态（P2_ATTACK_MOVE 起始，链完整）
    private List<MoveState> GeneratePhase2StartState()
    {
        var attack = new MoveState("P2_ATTACK_MOVE", P2AttackMove, new AttackIntentCustom(P2Hit, "ETERNATUS.intent.p2attack", Asc), new EffectIntentCustom("ETERNATUS.intent.p2awe", Asc));
        var defendBuff = new MoveState("P2_DEFEND_BUFF_MOVE", P2DefendBuffMove, new DefendIntentCustom("ETERNATUS.intent.p2defend", Asc), new BuffIntentCustom("ETERNATUS.intent.p2buff", Asc));
        var multiHit = new MoveState("P2_MULTI_MOVE", P2MultiMove, new AttackIntentCustom(P2MultiDmg, "ETERNATUS.intent.p2multi", Asc));
        var healDefend = new MoveState("P2_HEAL_MOVE", P2HealMove, new HealIntentCustom("ETERNATUS.intent.p2heal", Asc), new DefendIntentCustom("ETERNATUS.intent.p2healdefend", Asc));

        attack.FollowUpState = defendBuff;
        defendBuff.FollowUpState = multiHit;
        multiHit.FollowUpState = healDefend;
        healDefend.FollowUpState = attack;
        return new List<MoveState> { attack, defendBuff, multiHit, healDefend };
    }

    private MonsterMoveStateMachine GeneratePhase2()
    {
        var states = GeneratePhase2StartState();
        return new MonsterMoveStateMachine(states, states[0]);
    }

    private int BigHitDmg => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 25, 23);
    private int MultiHitDmg => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 4, 3);
    private int P2Hit => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 32, 30);
    private int P2MultiDmg => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 5, 4);
    private int P2HealAmt => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 50, 25);

    private async Task Status1Move(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackDebuffTrigger", 0.3f);
        var players = base.CombatState?.Players;
        if (players == null) return;
        foreach (var player in players)
        {
            for (int i = 0; i < 2; i++)
            {
                var beckon = base.CombatState!.CreateCard<MegaCrit.Sts2.Core.Models.Cards.Beckon>(player);
                await CardPileCmd.AddGeneratedCardsToCombat(new[] { beckon }, PileType.Draw, player);
            }
            for (int i = 0; i < 2; i++)
            {
                var dazed = base.CombatState!.CreateCard<MegaCrit.Sts2.Core.Models.Cards.Dazed>(player);
                await CardPileCmd.AddGeneratedCardsToCombat(new[] { dazed }, PileType.Draw, player);
            }
        }
    }

    private async Task BuffMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "BuffTrigger", 0.3f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
    }

    private async Task BigHitMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackHeavy", 0.25f);
        foreach (var t in targets)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, BigHitDmg, ValueProp.Unpowered, base.Creature, null);
        }
    }

    private async Task MultiHitMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackMulti", 0.3f);
        for (int i = 0; i < 3; i++)
        {
            foreach (var t in targets)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, MultiHitDmg, ValueProp.Unpowered, base.Creature, null);
            }
        }
    }

    private async Task DefendMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "BlockTrigger", 0.3f);
        await CreatureCmd.GainBlock(base.Creature, 40m, ValueProp.Unpowered, null);
    }

    private async Task P2AttackMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackHeavy", 0.25f);
        foreach (var t in targets)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, P2Hit, ValueProp.Unpowered, base.Creature, null);
        }
        foreach (var t in targets)
        {
            if (t.Player != null)
            {
                await PowerCmd.Apply<AwePower>(new ThrowingPlayerChoiceContext(), t, 1m, base.Creature, null);
            }
        }
    }

    private async Task P2DefendBuffMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackBlock", 0.3f);
        await CreatureCmd.GainBlock(base.Creature, 30m, ValueProp.Unpowered, null);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

    private async Task P2MultiMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackMulti", 0.35f);
        for (int i = 0; i < 4; i++)
        {
            foreach (var t in targets)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, P2MultiDmg, ValueProp.Unpowered, base.Creature, null);
            }
        }
    }

    private async Task P2HealMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "HealTrigger", 0.3f);
        await CreatureCmd.Heal(base.Creature, P2HealAmt, playAnim: true);
        await CreatureCmd.GainBlock(base.Creature, 20m, ValueProp.Unpowered, null);
    }
}
