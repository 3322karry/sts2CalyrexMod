using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CalyrexMod.Monsters;

// IV-1 巨锻匠 Tinkaton 80/80（88/88）：【攻（35（39））→晕】
public sealed class Tinkaton : LeagueMonsterBase
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 88, 80);
    public override int MaxInitialHp => MinInitialHp;

    protected override string VisualsPath => "res://CalyrexMod/monsters/tinkaton.tscn";

    private int Hit => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 39, 35);

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var attack = new MoveState("ATTACK_MOVE", AttackMove, new AttackIntentCustom(Hit, "TINKATON.intent.attack"));
        var stun = new MoveState("STUN_MOVE", StunMove, new StunIntentCustom("TINKATON.intent.stun"));

        attack.FollowUpState = stun;
        stun.FollowUpState = attack;

        return new MonsterMoveStateMachine(new List<MonsterState> { attack, stun }, attack);
    }

    private async Task AttackMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, Hit, ValueProp.Unpowered, base.Creature, null);
        }
    }

    private async Task StunMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await CreatureCmd.Stun(t, (_) => Task.CompletedTask);
        }
    }
}
