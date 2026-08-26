using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

using CalyrexMod.Monsters;

namespace CalyrexMod.Events;

// 尖塔宝可梦联赛战斗：I→III、II→IV 换人顺序
public sealed class PokemonLeagueEncounter : EncounterModel
{
    public override RoomType RoomType => RoomType.Monster;

    public override IReadOnlyList<string> Slots => new[] { "slot1", "slot2" };

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
        // 静态换人表：I→III、II→IV（IV 随机巨锻匠/谜拟丘）
        LeagueMonsterBase.RegisterNext(typeof(Incineroar), typeof(Toxapex));
        LeagueMonsterBase.RegisterNextRandom(typeof(Garchomp), typeof(Tinkaton), typeof(Mimikyu));

        var incineroar = (Incineroar)ModelDb.Monster<Incineroar>().ToMutable();
        var garchomp = (Garchomp)ModelDb.Monster<Garchomp>().ToMutable();

        return new (MonsterModel, string?)[]
        {
            (incineroar, "slot1"),
            (garchomp, "slot2")
        };
    }
}
