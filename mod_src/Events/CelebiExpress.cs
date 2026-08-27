using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using CalyrexMod.Characters;
using CalyrexMod.RelicPools;
using CalyrexMod.PotionPools;

namespace CalyrexMod.Events;

public sealed class CelebiExpress : EventModel
{
    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All((Player p) => p.Character is CalyrexCharacter);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, GivePotions, "CELEBI_EXPRESS.pages.INITIAL.options.GIVE_POTIONS"),
            new EventOption(this, GiveGold, "CELEBI_EXPRESS.pages.INITIAL.options.GIVE_GOLD")
        };
    }

    private async Task GiveRelic()
    {
        await GiveRandomExclusiveRelic();
        SetEventFinished(L10NLookup("CELEBI_EXPRESS.pages.DONE.description"));
    }

    private async Task GivePotions()
    {
        await GiveRandomExclusivePotions(2);
        SetEventFinished(L10NLookup("CELEBI_EXPRESS.pages.DONE.description"));
    }

    private async Task GiveGold()
    {
        await PlayerCmd.GainGold(100m, base.Owner);
        SetEventFinished(L10NLookup("CELEBI_EXPRESS.pages.DONE.description"));
    }

    private async Task GiveRandomExclusiveRelic()
    {
        var pool = ModelDb.RelicPool<CalyrexRelicPool>();
        var owned = base.Owner.Relics.Select((RelicModel r) => r.Id).ToHashSet();
        var candidates = pool.AllRelics.Where((RelicModel r) => !owned.Contains(r.Id)).ToList();
        if (candidates.Count == 0)
        {
            return;
        }
        RelicModel relic = candidates[base.Rng.NextInt(candidates.Count)].ToMutable();
        await RelicCmd.Obtain(relic, base.Owner);
    }

    private async Task GiveRandomExclusivePotions(int count)
    {
        var pool = ModelDb.PotionPool<CalyrexPotionPool>();
        var candidates = pool.AllPotions.ToList();
        for (int i = 0; i < count && candidates.Count > 0; i++)
        {
            int index = base.Rng.NextInt(candidates.Count);
            await PotionCmd.TryToProcure(candidates[index].ToMutable(), base.Owner);
            candidates.RemoveAt(index);
        }
    }
}
