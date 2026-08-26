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

// I 炽焰咆哮虎 Incineroar 90/90（99/99）：减（2力）→【攻（15（16））→攻（3（4））→增（自己1力）】
public sealed class Incineroar : LeagueMonsterBase
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 99, 90);
    public override int MaxInitialHp => MinInitialHp;

    protected override string VisualsPath => "res://CalyrexMod/monsters/incineroar.tscn";

    private int StrDebuff => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 2);
    private int BigHit => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 16, 15);
    private int SmallHit => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 4, 3);

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var debuff = new MoveState("DEBUFF_MOVE", DebuffMove, new DebuffIntentCustom("INCINEROAR.intent.debuff"));
        var big = new MoveState("BIG_HIT_MOVE", BigHitMove, new AttackIntentCustom(BigHit, "INCINEROAR.intent.bigHit"));
        var small = new MoveState("SMALL_HIT_MOVE", SmallHitMove, new AttackIntentCustom(SmallHit, "INCINEROAR.intent.smallHit"));
        var buff = new MoveState("BUFF_MOVE", BuffMove, new BuffIntentCustom("INCINEROAR.intent.buff"));

        debuff.FollowUpState = big;
        big.FollowUpState = small;
        small.FollowUpState = buff;
        buff.FollowUpState = big;

        return new MonsterMoveStateMachine(new List<MonsterState> { debuff, big, small, buff }, debuff);
    }

    private async Task DebuffMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), t, -StrDebuff, base.Creature, null);
        }
    }

    private async Task BigHitMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, BigHit, ValueProp.Unpowered, base.Creature, null);
        }
    }

    private async Task SmallHitMove(IReadOnlyList<Creature> targets)
    {
        foreach (var t in targets)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t, SmallHit, ValueProp.Unpowered, base.Creature, null);
        }
    }

    private async Task BuffMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }
}
