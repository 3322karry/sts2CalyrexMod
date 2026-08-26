using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using CalyrexMod.CardPools;
using CalyrexMod.Cards;
using CalyrexMod.PotionPools;
using CalyrexMod.RelicPools;
using CalyrexMod.Relics;

namespace CalyrexMod.Characters;

public sealed class CalyrexCharacter : CharacterModel
{
    public override Color NameColor => new Color("007A79");

    public override Color MapDrawingColor => new Color("007A79");

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override CharacterGender Gender => CharacterGender.Neutral;

    protected override CharacterModel? UnlocksAfterRunAs => null;

    public override int StartingHp => 75;

    public override int StartingGold => 99;

    public override CardPoolModel CardPool => ModelDb.CardPool<CalyrexCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<CalyrexRelicPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<CalyrexPotionPool>();

    public override IEnumerable<CardModel> StartingDeck => new CardModel[]
    {
        ModelDb.Card<CalyrexStrike>(),
        ModelDb.Card<CalyrexStrike>(),
        ModelDb.Card<CalyrexStrike>(),
        ModelDb.Card<CalyrexStrike>(),
        ModelDb.Card<CalyrexStrike>(),
        ModelDb.Card<CalyrexStrike>(),
        ModelDb.Card<CalyrexDefend>(),
        ModelDb.Card<CalyrexDefend>(),
        ModelDb.Card<CalyrexDefend>(),
        ModelDb.Card<CalyrexDefend>(),
        ModelDb.Card<CalyrexDefend>(),
        ModelDb.Card<CalyrexDefend>(),
        ModelDb.Card<DivineBlessing>(),
        ModelDb.Card<BondedReins>()
    };

    public override IReadOnlyList<RelicModel> StartingRelics => new[] { ModelDb.Relic<BlackWhiteCarrot>() };

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    protected override string IconPath => "res://CalyrexMod/icons/calyrex_icon.tscn";

    protected override string CharacterSelectIconPath => "res://images/packed/character_select/char_select_ironclad.png";

    protected override string CharacterSelectLockedIconPath => "res://images/packed/character_select/char_select_ironclad_locked.png";

    protected override string MapMarkerPath => "res://images/packed/map/icons/map_marker_ironclad.png";

    public override List<string> GetArchitectAttackVfx()
    {
        return new List<string>
        {
            "vfx/vfx_attack_blunt",
            "vfx/vfx_heavy_blunt",
            "vfx/vfx_attack_slash",
            "vfx/vfx_heavy_slash",
            "vfx/vfx_attack_thrust"
        };
    }
}
