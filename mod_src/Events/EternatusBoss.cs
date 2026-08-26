using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace CalyrexMod.Events;

// 无极汰那 Boss（荣耀幕）
public sealed class EternatusBoss : EncounterModel
{
    public override RoomType RoomType => RoomType.Boss;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => new MonsterModel[]
    {
        ModelDb.Monster<CalyrexMod.Monsters.Eternatus>()
    };

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        return new (MonsterModel, string?)[]
        {
            (ModelDb.Monster<CalyrexMod.Monsters.Eternatus>().ToMutable(), null)
        };
    }
}
