using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using CalyrexMod.Characters;
using CalyrexMod.Relics;
using MegaCrit.Sts2.Core.Commands;

namespace CalyrexMod.Events;

// 尖塔宝可梦联赛：进入战斗，成功后获得王者之证/特性膏药（随机）；或离开，下场战斗开始获得2虚弱
public sealed class PokemonLeague : EventModel
{
    public override bool IsShared => true;

    public override bool IsAllowed(IRunState runState)
    {
        // 荣耀（第 4 幕）专属 + 蕾冠王
        if (runState.CurrentActIndex != 3)
        {
            return false;
        }
        return runState.Players.All((Player p) => p.Character is CalyrexCharacter);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Enter, "POKEMON_LEAGUE.pages.INITIAL.options.ENTER"),
            new EventOption(this, Leave, "POKEMON_LEAGUE.pages.INITIAL.options.LEAVE")
        };
    }

    private async Task Enter()
    {
        // 进入战斗，胜利后获得随机遗物（王者之证/特性膏药）
        var relic = base.Rng.NextBool()
            ? ModelDb.Relic<KingRock>().ToMutable()
            : ModelDb.Relic<AbilityCapsule>().ToMutable();
        var rewards = new List<Reward> { new RelicReward(relic, base.Owner) };
        EnterCombatWithoutExitingEvent<PokemonLeagueEncounter>(rewards, shouldResumeAfterCombat: true);
        await Task.CompletedTask;
    }

    private async Task Leave()
    {
        // 离开：下场战斗开始时获得 2 虚弱
        await RelicCmd.Obtain(ModelDb.Relic<CalyrexMod.Relics.LeagueWeakRelic>().ToMutable(), base.Owner);
        SetEventFinished(L10NLookup("POKEMON_LEAGUE.pages.LEAVE.description"));
    }
}
