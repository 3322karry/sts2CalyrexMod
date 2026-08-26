using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using CalyrexMod.Characters;
using CalyrexMod.RelicPools;
using CalyrexMod.PotionPools;

namespace CalyrexMod.Events;

public sealed class PokemonDaycare : EventModel
{
    public override bool IsAllowed(IRunState runState)
    {
        if (runState.CurrentActIndex != 1)
        {
            return false;
        }
        return runState.Players.All((Player p) => p.Character is CalyrexCharacter);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, TakeEgg, "POKEMON_DAYCARE.pages.INITIAL.options.TAKE_EGG", HoverTipFactory.FromEnchantment<Glam>(2)),
            new EventOption(this, TakeStone, "POKEMON_DAYCARE.pages.INITIAL.options.TAKE_STONE"),
            new EventOption(this, TakeBag, "POKEMON_DAYCARE.pages.INITIAL.options.TAKE_BAG")
        };
    }

    private async Task TakeEgg()
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        EnchantmentModel enchantment = ModelDb.Enchantment<Glam>();
        CardModel card = (await CardSelectCmd.FromDeckForEnchantment(base.Owner, enchantment, 2, (CardModel? c) => enchantment.CanEnchant(c), prefs)).FirstOrDefault();
        if (card != null)
        {
            CardCmd.Enchant<Glam>(card, 2);
            NCardEnchantVfx vfx = NCardEnchantVfx.Create(card);
            if (vfx != null)
            {
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
            }
        }
        SetEventFinished(L10NLookup("POKEMON_DAYCARE.pages.EGG.description"));
    }

    private async Task TakeStone()
    {
        foreach (CardModel item in await CardSelectCmd.FromDeckForUpgrade(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 2)))
        {
            CardCmd.Upgrade(item);
        }
        SetEventFinished(L10NLookup("POKEMON_DAYCARE.pages.STONE.description"));
    }

    private async Task TakeBag()
    {
        if (base.Owner.Gold >= 100)
        {
            await PlayerCmd.LoseGold(100m, base.Owner);
            for (int i = 0; i < 3; i++)
            {
                await GiveRandomExclusiveRelic();
            }
        }
        SetEventFinished(L10NLookup("POKEMON_DAYCARE.pages.BAG.description"));
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
}
