# CalyrexMod 素材替换清单

所有自定义图片位于 `assets/`。替换同名 **PNG** 后运行部署脚本（自动重新生成 .tres / 重打包 pck）。

> 规则：`xxx.png` 与 `xxx.tres` 成对存在，.tres 是 .png 的 RGBA 内嵌（由脚本生成，无需手改）。
> 建议尺寸见各表；**方形图标**居中绘制、四周留边距；**场景形象**人物居中、脚部贴底。

---

## 1. 角色形象（PNG 475×475，透明背景，脚贴底部）

| 文件 | 用途 | 当前来源 |
|---|---|---|
| `assets/scenes/calyrex.tscn` | 战斗中蕾冠王形象（含内嵌图） | 游戏原始立绘 |
| `assets/scenes/calyrex_ice.tscn` | 骑雪暴马（冰马）合体形象 | 游戏原始立绘 |
| `assets/scenes/calyrex_shadow.tscn` | 骑灵幽马（黑马）合体形象 | 游戏原始立绘 |
| `assets/scenes/glastrier.tscn` | 雪暴马宠物形象 | PokeAPI 官方 |
| `assets/scenes/spectrier.tscn` | 灵幽马宠物形象 | PokeAPI 官方 |
| `assets/scenes/calyrex_merchant.tscn` | **商店**蕾冠王形象（新） | 复用角色图 |
| `assets/scenes/calyrex_rest_site.tscn` | **火堆**蕾冠王形象（新） | 复用角色图 |

> 注：merchant/rest_site 场景内嵌同一张角色图，替换时可直接改场景内 `PackedByteArray`，或提供两张独立 PNG（如 `calyrex_merchant.png` / `calyrex_rest_site.png`）后更新场景。

## 2. 角色选择

| 文件 | 用途 | 建议尺寸 |
|---|---|---|
| `assets/icons/calyrex_icon.png` | 角色头像/图标 | 128×128 |
| `assets/icons/char_select_calyrex.png` | 角色选择按钮大头像 | 475×475 |
| `assets/scenes/char_select_bg_calyrex.png` | 角色选择**背景**（用户已绘制） | 2560×1200 |

## 3. 事件背景（用户已绘制 3440×1613 → 缩放 2560×1200）

| 文件 | 用途 |
|---|---|
| `assets/icons/events/celebi_express.png` | 时拉比速递事件背景 |
| `assets/icons/events/pokemon_daycare.png` | 宝可梦培育屋事件背景 |

## 4. 能量图标 / 卡框

| 文件 | 用途 |
|---|---|
| `assets/icons/energy_calyrex.png` | 蕾冠王能量图标（费用）128×128 |
| `assets/icons/card_frame_calyrex_mat.tres` | 卡框材质（HSV 调色，非图片） |

## 5. 标记图标（Power，128×128，当前为几何绘制，可替换为手绘）

`assets/icons/markers/`：
`abundance`（丰饶）、`accelerate`（加速）、`cannot_mount`、`dynamax_form`（极巨化）、`encore`（再来一次）、`eternal_whinny`（永恒嘶鸣）、`friendly_guard`、`frozen`（冰冻）、`future_sight`（预知未来）、`grassy_terrain`（青草场地）、`heavy_lance`（重装之矛）、`helping_hand`（帮手）、`ice_wall`（冰墙）、`ingrain`（扎根）、`leech_seed`（寄生种子）、`mounted_glastrier`、`mounted_spectrier`、`pp_restore`、`pressure`（压迫感）、`quick_sight`（迅疾之视）、`slow_down`（减速）、`soul_heart`（魂心）、`steed_guard`（骏马守护）、`temp_thorns`、`trick`（戏法）、`truce`（休战）、`courage_rope`（勇气绳索）

## 6. 遗物图标（128×128）

| 文件 | 来源 |
|---|---|
| `assets/icons/relics/focus-sash.png` 等官方道具 | PokeAPI 官方物品图（focus-sash/never-melt-ice/spell-tag/eviolite/exp-share/miracle-seed） |
| `assets/icons/relics/loaded_dice.png` | 用户提供（机变骰子） |
| `assets/icons/relics/black_white_carrot.png` 等 | 几何绘制（black_white_carrot/multiscale/snow_carrot/serene_grace/disguise） |

全部：`black_white_carrot`、`disguise`、`eviolite`、`exp-share`、`focus-sash`、`loaded_dice`、`miracle-seed`、`multiscale`、`never-melt-ice`、`serene_grace`、`snow_carrot`、`spell-tag`

## 7. 药水图标（128×128）

| 文件 | 来源 |
|---|---|
| `assets/icons/potions/figy_berry.png` | PokeAPI 官方（用户提供图替换过） |
| `gray_carrot` / `defense_boost` / `galarian_spice` / `victors_curry` / `plated_armor` | 几何绘制 |

## 8. 其他（非图片）

- `src/ModInfo.cs`：版本号
- `assets/localization/{eng,zhs}/*.json`：全部文本
- 卡牌立绘：目前用游戏默认卡面（`card_atlas` fallback），如需专属卡面需额外方案

---

## 9. 卡面（Card Portrait）

**尺寸：1000×760（横向）**。游戏从 `res://images/atlases/card_atlas.sprites/calyrex/{slug}.tres` 读取卡面，
当前缺失时回退到通用卡面。

> 用法：将 `{slug}.png` 放入 `assets/icons/card_portraits/`（如 `calyrex_strike.png`），
> 部署时由脚本生成 .tres 并模拟进 atlas 路径打进 pck。

> ⭐ = **先古卡**（`CardRarity.Ancient`，由达弗/古旧之书给予）

全部 69 张卡（类名 → 卡面 slug）：

| 类名 | slug |
|---|---|
| `AbsoluteZero` | `absolute_zero` |
| `Accelerate` | `accelerate` |
| `AllOutAttack` | `all_out_attack` |
| `Anxious` | `anxious` |
| `BatonPass` | `baton_pass` |
| `BondedReins` | `bonded_reins` |
| `Boomburst` | `boomburst` |
| `CalyrexDefend` | `calyrex_defend` |
| `CalyrexStrike` | `calyrex_strike` |
| `CatastrophicBlow` | `catastrophic_blow` |
| `CleanUpFirst` | `clean_up_first` |
| `CourageRope` ⭐ | `courage_rope` |
| `CrownTundra` | `crown_tundra` |
| `DarkGleam` | `dark_gleam` |
| `DefendPosition` | `defend_position` |
| `DivineBlessing` | `divine_blessing` |
| `DynamaxForm` | `dynamax_form` |
| `Encore` ⭐ | `encore` |
| `ExtremeSpeed` | `extreme_speed` |
| `FakeOut` | `fake_out` |
| `FriendlyGuard` | `friendly_guard` |
| `Frost` | `frost` |
| `FutureSight` | `future_sight` |
| `Gallop` | `gallop` |
| `GlacialWorld` | `glacial_world` |
| `GrassKnot` | `grass_knot` |
| `GrassyGlide` | `grassy_glide` |
| `GrassyTerrain` | `grassy_terrain` |
| `Harvest` | `harvest` |
| `HelpingHand` | `helping_hand` |
| `HeroicSacrifice` | `heroic_sacrifice` |
| `HighHorsepower` | `high_horsepower` |
| `HorseLove` | `horse_love` |
| `IcyWind` | `icy_wind` |
| `Ingrain` | `ingrain` |
| `IntensivePlanting` | `intensive_planting` |
| `Intimidate` | `intimidate` |
| `LeechSeed` | `leech_seed` |
| `LonePath` | `lone_path` |
| `LongHowl` | `long_howl` |
| `MaleficCurse` | `malefic_curse` |
| `PPRestoreCard` | `pp_restore_card` |
| `PaleLance` | `pale_lance` |
| `PartingShot` | `parting_shot` |
| `PhantomForce` | `phantom_force` |
| `PlantCarrot` | `plant_carrot` |
| `Pressure` | `pressure` |
| `Protect` | `protect` |
| `Psychic` | `psychic` |
| `QuickAttack` | `quick_attack` |
| `Recall` | `recall` |
| `RoyalFavor` | `royal_favor` |
| `SeedBomb` | `seed_bomb` |
| `ShadowBall` | `shadow_ball` |
| `SlowDown` | `slow_down` |
| `SoulBlast` | `soul_blast` |
| `SoulHeart` | `soul_heart` |
| `SpikyDefense` | `spiky_defense` |
| `StoredPower` | `stored_power` |
| `StrengthSap` | `strength_sap` |
| `Substitute` | `substitute` |
| `TeraBlast` | `tera_blast` |
| `Tribute` | `tribute` |
| `Trick` | `trick` |
| `Truce` | `truce` |
| `UproarRoar` | `uproar_roar` |
| `WideGuard` | `wide_guard` |
| `WishGrant` | `wish_grant` |
| `ZenHeadbutt` | `zen_headbutt` |
