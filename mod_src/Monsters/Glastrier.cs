using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace CalyrexMod.Monsters;

public sealed class Glastrier : MonsterModel
{
    public override int MinInitialHp => 0;

    public override int MaxInitialHp => 0;

    public override bool IsHealthBarVisible => true;

    protected override string VisualsPath => "res://CalyrexMod/scenes/glastrier.tscn";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        MoveState moveState = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
        moveState.FollowUpState = moveState;
        return new MonsterMoveStateMachine(new MonsterState[] { moveState }, moveState);
    }
}
