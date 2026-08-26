using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Monsters;

// II 烈咬陆鲨 Garchomp 108/108（118/118）：
// 【增（自己2力）→攻（5*2（4*3））→牌（2晕眩，抽牌堆）】
public sealed class Garchomp : LeagueMonsterBase
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 118, 108);
    public override int MaxInitialHp => MinInitialHp;

        private bool Asc => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 1, 0) > 0;

    protected override string VisualsPath => "res://CalyrexMod/monsters/garchomp.tscn";

    private int BuffAmount => 2;
    private int HitDmg => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 4, 5);
    private int HitCount => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 3, 2);

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var buff = new MoveState("BUFF_MOVE", BuffMove, new BuffIntentCustom("GARCHOMP.intent.buff", Asc));
        var attack = new MoveState("ATTACK_MOVE", AttackMove, new AttackIntentCustom(HitDmg, "GARCHOMP.intent.attack", Asc));
        var status = new MoveState("STATUS_MOVE", StatusMove, new StatusIntentCustom("GARCHOMP.intent.status", Asc));

        buff.FollowUpState = attack;
        attack.FollowUpState = status;
        status.FollowUpState = buff;

        return new MonsterMoveStateMachine(new List<MonsterState> { buff, attack, status }, buff);
    }

    private async Task BuffMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "BuffTrigger", 0.3f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, BuffAmount, base.Creature, null);
    }

    private async Task AttackMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackMulti", 0.3f);
        for (int i = 0; i < HitCount; i++)
        {
            foreach (var t in targets)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, HitDmg, ValueProp.Unpowered, base.Creature, null);
            }
        }
    }

    private async Task StatusMove(IReadOnlyList<Creature> targets)
    {
        // 洗 2 张晕眩到抽牌堆
        var combatState = base.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }
        var players = combatState.Players;
        foreach (var player in players)
        {
            for (int i = 0; i < 2; i++)
            {
                var stun = combatState.CreateCard<MegaCrit.Sts2.Core.Models.Cards.Dazed>(player);
                await CardPileCmd.AddGeneratedCardsToCombat(new[] { stun }, PileType.Draw, player);
            }
        }
    }
}
