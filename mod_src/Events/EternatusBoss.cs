using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace CalyrexMod.Events;

// 无极汰那 Boss（荣耀幕）
public sealed class EternatusBoss : EncounterModel
{
    public override RoomType RoomType => RoomType.Boss;

    // 无专属 spine 资源：地图 Boss 节点走 placeholder 图片分支
    public override MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSkeletonDataResource? BossNodeSpineResource => null;

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
