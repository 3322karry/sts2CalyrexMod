using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

using CalyrexMod.Monsters;

namespace CalyrexMod.Events;

// 尖塔宝可梦联赛战斗：I→III、II→IV 换人顺序
public sealed class PokemonLeagueEncounter : EncounterModel
{
    public override RoomType RoomType => RoomType.Monster;

    public override IEnumerable<MonsterModel> AllPossibleMonsters => new MonsterModel[]
    {
        ModelDb.Monster<CalyrexMod.Monsters.Incineroar>(),
        ModelDb.Monster<CalyrexMod.Monsters.Garchomp>(),
        ModelDb.Monster<CalyrexMod.Monsters.Toxapex>(),
        ModelDb.Monster<CalyrexMod.Monsters.Tinkaton>(),
        ModelDb.Monster<CalyrexMod.Monsters.Mimikyu>()
    };

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        var incineroar = (CalyrexMod.Monsters.Incineroar)ModelDb.Monster<CalyrexMod.Monsters.Incineroar>().ToMutable();
        var garchomp = (CalyrexMod.Monsters.Garchomp)ModelDb.Monster<CalyrexMod.Monsters.Garchomp>().ToMutable();
        var toxapex = ModelDb.Monster<CalyrexMod.Monsters.Toxapex>().ToMutable();

        // 换人顺序：I→III、II→IV（IV 随机巨锻匠/谜拟丘）
        incineroar.NextMonsterType = typeof(CalyrexMod.Monsters.Toxapex);
        garchomp.NextMonsterType = Rng.NextBool() ? typeof(CalyrexMod.Monsters.Tinkaton) : typeof(CalyrexMod.Monsters.Mimikyu);

        return new (MonsterModel, string?)[]
        {
            (incineroar, "slot1"),
            (garchomp, "slot2")
        };
    }
}
