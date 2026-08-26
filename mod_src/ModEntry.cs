using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves.Runs;
using CalyrexMod.CardPools;
using MegaCrit.Sts2.Core.Models.CardPools;
using CalyrexMod.Cards;

namespace CalyrexMod;

[ModInitializer(nameof(Initialize))]
public static class ModEntry
{
    private static Harmony? _harmony;

    public static void Initialize()
    {
        Stage1Harmony();
        Stage2Register();
        Stage3Settings();

        Log.Info($"[{ModInfo.ModId}] {ModInfo.ModName} {ModInfo.Version} loaded!");
        try
        {
            var hasImagePatch = HarmonyLib.AccessTools.Method(typeof(PotionModel), "get_Image") != null;
            bool potionPatched = HarmonyLib.Harmony.GetAllPatchedMethods()
                .Any((System.Reflection.MethodBase m) => m.DeclaringType == typeof(PotionModel));
            Log.Info($"[{ModInfo.ModId}] check: get_Image exists={hasImagePatch}, PotionModel patched methods={potionPatched}");
            if (potionPatched)
            {
                foreach (var m in HarmonyLib.Harmony.GetAllPatchedMethods().Where((System.Reflection.MethodBase m) => m.DeclaringType == typeof(PotionModel)))
                {
                    Log.Info($"[{ModInfo.ModId}] patched: {m.Name}");
                }
            }
        }
        catch (System.Exception ex)
        {
            Log.Info($"[{ModInfo.ModId}] check failed: {ex.Message}");
        }
    }

    private static void Stage1Harmony()
    {
        try
        {
            _harmony = new Harmony($"com.vibeprograms.{ModInfo.ModId}");
            _harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Log.Error($"[{ModInfo.ModId}] Harmony init failed: {ex}");
        }
    }

    private static void Stage2Register()
    {
        try
        {
            // 卡池：第一批
            ModHelper.AddModelToPool<CalyrexCardPool, SpikyDefense>();
            ModHelper.AddModelToPool<CalyrexCardPool, GrassKnot>();
            ModHelper.AddModelToPool<CalyrexCardPool, Protect>();
            ModHelper.AddModelToPool<CalyrexCardPool, DynamaxForm>();
            ModHelper.AddModelToPool<CalyrexCardPool, BatonPass>();
            ModHelper.AddModelToPool<CalyrexCardPool, HelpingHand>();
            // 卡池：第二批
            ModHelper.AddModelToPool<CalyrexCardPool, StrengthSap>();
            ModHelper.AddModelToPool<CalyrexCardPool, SoulBlast>();
            ModHelper.AddModelToPool<CalyrexCardPool, Boomburst>();
            ModHelper.AddModelToPool<CalyrexCardPool, QuickAttack>();
            ModHelper.AddModelToPool<CalyrexCardPool, GrassyTerrain>();
            ModHelper.AddModelToPool<CalyrexCardPool, GrassyGlide>();
            ModHelper.AddModelToPool<CalyrexCardPool, HeroicSacrifice>();
            ModHelper.AddModelToPool<CalyrexCardPool, Tribute>();
            // 卡池：第三批
            ModHelper.AddModelToPool<CalyrexCardPool, Gallop>();
            ModHelper.AddModelToPool<CalyrexCardPool, Substitute>();
            ModHelper.AddModelToPool<CalyrexCardPool, HighHorsepower>();
            ModHelper.AddModelToPool<CalyrexCardPool, Truce>();
            ModHelper.AddModelToPool<CalyrexCardPool, TeraBlast>();
            ModHelper.AddModelToPool<CalyrexCardPool, CleanUpFirst>();
            ModHelper.AddModelToPool<CalyrexCardPool, RoyalFavor>();
            // 卡池：第四批
            ModHelper.AddModelToPool<CalyrexCardPool, DarkGleam>();
            ModHelper.AddModelToPool<CalyrexCardPool, PaleLance>();
            ModHelper.AddModelToPool<CalyrexCardPool, StoredPower>();
            ModHelper.AddModelToPool<CalyrexCardPool, Recall>();
            ModHelper.AddModelToPool<CalyrexCardPool, FutureSight>();
            ModHelper.AddModelToPool<CalyrexCardPool, Psychic>();
            ModHelper.AddModelToPool<CalyrexCardPool, UproarRoar>();
            // 卡池：第五批
            ModHelper.AddModelToPool<CalyrexCardPool, PlantCarrot>();
            ModHelper.AddModelToPool<CalyrexCardPool, Accelerate>();
            ModHelper.AddModelToPool<CalyrexCardPool, SlowDown>();
            ModHelper.AddModelToPool<CalyrexCardPool, ExtremeSpeed>();
            ModHelper.AddModelToPool<CalyrexCardPool, DefendPosition>();
            ModHelper.AddModelToPool<CalyrexCardPool, ZenHeadbutt>();
            ModHelper.AddModelToPool<CalyrexCardPool, Anxious>();
            ModHelper.AddModelToPool<CalyrexCardPool, LonePath>();
            ModHelper.AddModelToPool<CalyrexCardPool, SeedBomb>();
            ModHelper.AddModelToPool<CalyrexCardPool, PartingShot>();
            ModHelper.AddModelToPool<CalyrexCardPool, CatastrophicBlow>();
            ModHelper.AddModelToPool<CalyrexCardPool, FakeOut>();
            ModHelper.AddModelToPool<CalyrexCardPool, PhantomForce>();
            ModHelper.AddModelToPool<CalyrexCardPool, Frost>();
            // 卡池：第六批
            ModHelper.AddModelToPool<CalyrexCardPool, CrownTundra>();
            ModHelper.AddModelToPool<CalyrexCardPool, LeechSeed>();
            ModHelper.AddModelToPool<CalyrexCardPool, HorseLove>();
            ModHelper.AddModelToPool<CalyrexCardPool, AllOutAttack>();
            ModHelper.AddModelToPool<CalyrexCardPool, Harvest>();
            ModHelper.AddModelToPool<CalyrexCardPool, IntensivePlanting>();
            ModHelper.AddModelToPool<CalyrexCardPool, Intimidate>();
            ModHelper.AddModelToPool<CalyrexCardPool, Pressure>();
            ModHelper.AddModelToPool<CalyrexCardPool, MaleficCurse>();
            ModHelper.AddModelToPool<CalyrexCardPool, IcyWind>();
            ModHelper.AddModelToPool<CalyrexCardPool, Ingrain>();
            ModHelper.AddModelToPool<CalyrexCardPool, SoulHeart>();
            ModHelper.AddModelToPool<CalyrexCardPool, Trick>();
            ModHelper.AddModelToPool<CalyrexCardPool, GlacialWorld>();
            ModHelper.AddModelToPool<CalyrexCardPool, AbsoluteZero>();
            ModHelper.AddModelToPool<CalyrexCardPool, WishGrant>();
            ModHelper.AddModelToPool<CalyrexCardPool, ShadowBall>();
            ModHelper.AddModelToPool<CalyrexCardPool, PPRestoreCard>();
            // 卡池：第七批（联机专属）
            ModHelper.AddModelToPool<CalyrexCardPool, WideGuard>();
            ModHelper.AddModelToPool<CalyrexCardPool, LongHowl>();
            ModHelper.AddModelToPool<CalyrexCardPool, FriendlyGuard>();
            // 卡池：先古牌（达弗给予）
            ModHelper.AddModelToPool<CalyrexCardPool, CourageRope>();
            ModHelper.AddModelToPool<CalyrexCardPool, Encore>();
            ModHelper.AddModelToPool<CalyrexCardPool, DebugCard>();
            // 卡池：第八批
            ModHelper.AddModelToPool<CalyrexCardPool, CalyrexHaze>();
            ModHelper.AddModelToPool<CalyrexCardPool, IronDefense>();
            ModHelper.AddModelToPool<CalyrexCardPool, Fly>();
            ModHelper.AddModelToPool<CalyrexCardPool, PollenPuff>();
            ModHelper.AddModelToPool<CalyrexCardPool, Defiant>();
            ModHelper.AddModelToPool<CalyrexCardPool, DracoMeteor>();
            ModHelper.AddModelToPool<CalyrexCardPool, LastResort>();
            ModHelper.AddModelToPool<CalyrexCardPool, SandAttack>();
            ModHelper.AddModelToPool<CalyrexCardPool, CalyrexPounce>();
            ModHelper.AddModelToPool<CalyrexCardPool, EchoingWhinny>();
            ModHelper.AddModelToPool<CalyrexCardPool, ZCelebrate>();
            ModHelper.AddModelToPool<CalyrexCardPool, PsychUp>();
            ModHelper.AddModelToPool<CalyrexCardPool, Spotlight>();
            ModHelper.AddModelToPool<CalyrexCardPool, MoveRecord>();
            ModHelper.AddModelToPool<CalyrexCardPool, IcicleCrash>();
            ModHelper.AddModelToPool<CalyrexCardPool, TripleAxel>();
            ModHelper.AddModelToPool<CalyrexCardPool, PsychicNoise>();
            ModHelper.AddModelToPool<ColorlessCardPool, Zero>();
        }
        catch (Exception ex)
        {
            Log.Error($"[{ModInfo.ModId}] Model registration failed: {ex}");
        }
    }

    private static void Stage3Settings()
    {
        try
        {
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.BlackWhiteCarrot));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.SereneGrace));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.FocusSash));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.NeverMeltIce));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.SpellTag));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.Eviolite));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.Multiscale));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.ExpShare));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.MiracleSeed));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.LoadedDice));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.Disguise));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.SnowCarrot));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.KingRock));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.AbilityCapsule));
            SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(CalyrexMod.Relics.LeagueWeakRelic));
        }
        catch (Exception ex)
        {
            Log.Error($"[{ModInfo.ModId}] Settings/serialization init failed: {ex}");
        }
    }
}
