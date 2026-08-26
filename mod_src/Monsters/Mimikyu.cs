using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Monsters;

// IV-2 谜拟丘 Mimikyu 60/60，自带 1（2）层缓冲：
// 增（5力）→【攻（1*3（1*4））→攻（10（15）），回复未被格挡伤害的血量→攻（1*4（1*5）），失去未被格挡伤害两倍的血量】
public sealed class Mimikyu : LeagueMonsterBase
{
    public override int MinInitialHp => 60;
    public override int MaxInitialHp => 60;

        private bool Asc => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 1, 0) > 0;

    protected override string VisualsPath => "res://CalyrexMod/monsters/mimikyu.tscn";

    private int BufferAmt => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);
    private int Hit1Count => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 4, 3);
    private int Hit2Dmg => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 15, 10);
    private int Hit3Count => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 5, 4);

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var buff = new MoveState("BUFF_MOVE", BuffMove, new BuffIntentCustom("MIMIKYU.intent.buff", Asc));
        var attack1 = new MoveState("ATTACK1_MOVE", Attack1Move, new AttackIntentCustom(1, "MIMIKYU.intent.attack1", Asc));
        var attack2 = new MoveState("ATTACK2_MOVE", Attack2Move, new AttackIntentCustom(Hit2Dmg, "MIMIKYU.intent.attack2", Asc));
        var attack3 = new MoveState("ATTACK3_MOVE", Attack3Move, new AttackIntentCustom(1, "MIMIKYU.intent.attack3", Asc));

        buff.FollowUpState = attack1;
        attack1.FollowUpState = attack2;
        attack2.FollowUpState = attack3;
        attack3.FollowUpState = attack1;

        return new MonsterMoveStateMachine(new List<MonsterState> { buff, attack1, attack2, attack3 }, buff);
    }

    public override async Task AfterAddedToRoom()
    {
        await PowerCmd.Apply<BufferPower>(new ThrowingPlayerChoiceContext(), base.Creature, BufferAmt, base.Creature, null);
    }

    private async Task BuffMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "BuffTrigger", 0.3f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 5m, base.Creature, null);
    }

    private async Task Attack1Move(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackMulti", 0.3f);
        for (int i = 0; i < Hit1Count; i++)
        {
            foreach (var t in targets)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, 1m, ValueProp.Unpowered, base.Creature, null);
            }
        }
    }

    private async Task Attack2Move(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackSingle", 0.2f);
        foreach (var t in targets)
        {
            var result = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, Hit2Dmg, ValueProp.Unpowered, base.Creature, null);
            // 回复未被格挡伤害的血量
            decimal dealt = 0;
            foreach (var r in result)
            {
                dealt += r.TotalDamage;
            }
            if (dealt > 0)
            {
                await CreatureCmd.Heal(base.Creature, dealt, playAnim: false);
            }
        }
    }

    private async Task Attack3Move(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(base.Creature, "AttackSingle", 0.2f);
        foreach (var t in targets)
        {
            var result = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, 1m, ValueProp.Unpowered, base.Creature, null);
            decimal dealt = 0;
            foreach (var r in result)
            {
                dealt += r.TotalDamage;
            }
            // 失去未被格挡伤害两倍的血量
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Creature, dealt * 2m, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        }
        for (int i = 0; i < Hit3Count; i++)
        {
            foreach (var t in targets)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, 1m, ValueProp.Unpowered, base.Creature, null);
            }
        }
    }
}
