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

// III 超坏星 Toxapex 55/55（60/60）：
// 减（2力）效（5（6）中毒）→【增（1力）→攻（5（7））→防（自己99）回（15（20））】
public sealed class Toxapex : LeagueMonsterBase
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 60, 55);
    public override int MaxInitialHp => MinInitialHp;

    protected override string VisualsPath => "res://CalyrexMod/monsters/toxapex.tscn";

    private int StrDebuff => 2;
    private int PoisonAmt => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 6, 5);
    private int Hit => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 7, 5);
    private int HealAmt => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 20, 15);
    private const int DefendAmt = 99;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var debuff = new MoveState("DEBUFF_MOVE", DebuffMove, new DebuffIntentCustom("TOXAPEX.intent.debuff"));
        var buff = new MoveState("BUFF_MOVE", BuffMove, new BuffIntentCustom("TOXAPEX.intent.buff"));
        var attack = new MoveState("ATTACK_MOVE", AttackMove, new AttackIntentCustom(Hit, "TOXAPEX.intent.attack"));
        var defend = new MoveState("DEFEND_MOVE", DefendMove, new DefendIntentCustom("TOXAPEX.intent.defend"), new HealIntentCustom("TOXAPEX.intent.heal"));

        debuff.FollowUpState = buff;
        buff.FollowUpState = attack;
        attack.FollowUpState = defend;
        defend.FollowUpState = buff;

        return new MonsterMoveStateMachine(new List<MonsterState> { debuff, buff, attack, defend }, debuff);
    }

    private async Task DebuffMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), t, -StrDebuff, base.Creature, null);
            await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), t, PoisonAmt, base.Creature, null);
        }
    }

    private async Task BuffMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

    private async Task AttackMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, Hit, ValueProp.Unpowered, base.Creature, null);
        }
    }

    private async Task DefendMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.GainBlock(base.Creature, DefendAmt, ValueProp.Unpowered, null);
        await CreatureCmd.Heal(base.Creature, HealAmt, playAnim: true);
    }
}
