using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using CalyrexMod.Powers;

namespace CalyrexMod.Monsters;

// 无极汰那 Eternatus（荣耀 Boss，两阶段）：
// 阶段1 368(408)：无极巨化；死亡后下回合复活并进入阶段2（消除自身所有效果）；血量<170 眩晕一回合
// 阶段2 768(828)：混乱（破盾给虚弱/易伤/脆弱）；震慑（本回合玩家只能打一张攻击牌）
public sealed class Eternatus : MonsterModel
{
    private bool _isPhase2;
    private bool _revivingNextTurn;
    private bool _stunPending;
    private const int Phase1Hp = 368;
    private const int Phase1HpAsc = 408;
    private const int Phase2Hp = 768;
    private const int Phase2HpAsc = 828;
    private const int StunThreshold = 170;

    public override int MinInitialHp => _isPhase2
        ? AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, Phase2HpAsc, Phase2Hp)
        : AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, Phase1HpAsc, Phase1Hp);

    public override int MaxInitialHp => MinInitialHp;

    private bool Asc => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 1, 0) > 0;

    protected override string VisualsPath => _isPhase2
        ? "res://CalyrexMod/monsters/eternatus_eternamax.tscn"
        : "res://CalyrexMod/monsters/eternatus.tscn";

    // 阶段1 buff
    public bool IsPhase2 => _isPhase2;

    // 死亡后留场（可复活）
    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return false;
    }


    // 死亡：阶段1 → 下回合复活进阶段2
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
        if (!_isPhase2 && !wasRemovalPrevented)
        {
            _revivingNextTurn = true;
            MegaCrit.Sts2.Core.Logging.Log.Info("[CalyrexMod] Eternatus phase1 died, reviving next turn");
        }
    }

    // 下回合开始：复活进阶段2
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Enemy && _revivingNextTurn && base.Creature.IsDead)
        {
            _revivingNextTurn = false;
            _isPhase2 = true;
            // 消除自身所有效果
            foreach (var p in base.Creature.Powers.ToList())
            {
                await PowerCmd.Remove(p);
            }
            // 复活 + 阶段2 血量 + 混乱/震慑 buff
            await CreatureCmd.SetCurrentHp(base.Creature, MinInitialHp);
            await PowerCmd.Apply<PanicPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
            await PowerCmd.Apply<EternamaxPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
            // 刷新视觉（无极巨化）：重新创建视觉
            var node = NCombatRoom.Instance?.GetCreatureNode(base.Creature);
            if (node != null)
            {
                var newVisuals = PreloadManager.Cache.GetScene(VisualsPath).Instantiate<NCreatureVisuals>(PackedScene.GenEditState.Disabled);
                newVisuals.Name = "Visuals";
                var oldVis = node.GetNodeOrNull<NCreatureVisuals>("Visuals");
                if (oldVis != null) node.RemoveChildSafely(oldVis);
                node.AddChildSafely(newVisuals);
                node.MoveChildSafely(newVisuals, 0);
            }
            MegaCrit.Sts2.Core.Logging.Log.Info("[CalyrexMod] Eternatus phase2 revived!");
        }
        await Task.CompletedTask;
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

    // 阶段1 意图：牌2（呼唤，抽牌堆）牌2（晕眩，抽牌堆）增（2力）→【攻（23（25））→增（1力）→攻（7*3（7*4），镰刀）→防40】
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

        return new MonsterMoveStateMachine(new List<MonsterState> { status1, buff, bigHit, multiHit, defend }, status1);
    }

    // 阶段2 意图：【攻30（32）效（1层震慑）→防30 增（1力）→攻7*4（7*5）→回25（50）防20】
    private MonsterMoveStateMachine GeneratePhase2()
    {
        var attack = new MoveState("P2_ATTACK_MOVE", P2AttackMove, new AttackIntentCustom(P2Hit, "ETERNATUS.intent.p2attack", Asc), new EffectIntentCustom("ETERNATUS.intent.p2awe", Asc));
        var defendBuff = new MoveState("P2_DEFEND_BUFF_MOVE", P2DefendBuffMove, new DefendIntentCustom("ETERNATUS.intent.p2defend", Asc), new BuffIntentCustom("ETERNATUS.intent.p2buff", Asc));
        var multiHit = new MoveState("P2_MULTI_MOVE", P2MultiMove, new AttackIntentCustom(P2MultiDmg, "ETERNATUS.intent.p2multi", Asc));
        var healDefend = new MoveState("P2_HEAL_MOVE", P2HealMove, new HealIntentCustom("ETERNATUS.intent.p2heal", Asc), new DefendIntentCustom("ETERNATUS.intent.p2healdefend", Asc));

        attack.FollowUpState = defendBuff;
        defendBuff.FollowUpState = multiHit;
        multiHit.FollowUpState = healDefend;
        healDefend.FollowUpState = attack;

        return new MonsterMoveStateMachine(new List<MonsterState> { attack, defendBuff, multiHit, healDefend }, attack);
    }

    private int BigHitDmg => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 25, 23);
    private int MultiHitDmg => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 4, 3);
    private int P2Hit => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 32, 30);
    private int P2MultiDmg => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 5, 4);
    private int P2HealAmt => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 50, 25);

    private async Task Status1Move(IReadOnlyList<Creature> targets)
    {
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
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 2m, base.Creature, null);
    }

    private async Task BigHitMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, BigHitDmg, ValueProp.Unpowered, base.Creature, null);
        }
    }

    private async Task MultiHitMove(IReadOnlyList<Creature> targets)
    {
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
        await CreatureCmd.GainBlock(base.Creature, 40m, ValueProp.Unpowered, null);
    }

    private async Task P2AttackMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, P2Hit, ValueProp.Unpowered, base.Creature, null);
        }
        // 震慑：本回合玩家只能打一张攻击牌
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
        await CreatureCmd.GainBlock(base.Creature, 30m, ValueProp.Unpowered, null);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

    private async Task P2MultiMove(IReadOnlyList<Creature> targets)
    {
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
        await CreatureCmd.Heal(base.Creature, P2HealAmt, playAnim: true);
        await CreatureCmd.GainBlock(base.Creature, 20m, ValueProp.Unpowered, null);
    }
}
