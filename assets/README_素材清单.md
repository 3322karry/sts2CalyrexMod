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

**背景要求**：卡面必须使用**不透明背景**（纯白或美术背景均可，**不能是透明/PNG Alpha 通道**——游戏卡面底板是暗色纹理，透明图会显示异常）。小尺寸图会被脚本等比放大并居中铺白底，但会糊，**正式素材请直接按 1000×760 出图**。

> 用法：将 `{slug}.png` 放入 `assets/icons/card_portraits/`（如 `calyrex_strike.png`），
> 部署时由脚本生成 .tres 并模拟进 atlas 路径打进 pck。

> ⭐ = **先古卡**（`CardRarity.Ancient`，由达弗/古旧之书给予）

全部 88 张卡（类名 → 卡面 slug → 中文名）：

| 类名 | slug | 中文名 |
|---|---|---|
| `AbsoluteZero` | `absolute_zero` | 绝对零度 |
| `Accelerate` | `accelerate` | 加速！！ |
| `AllOutAttack` | `all_out_attack` | 全力一击 |
| `Anxious` | `anxious` | 焦急 |
| `BatonPass` | `baton_pass` | 接棒 |
| `BondedReins` | `bonded_reins` | 牵绊缰绳 |
| `Boomburst` | `boomburst` | 爆音波 |
| `CalyrexDefend` | `calyrex_defend` | 防御 |
| `CalyrexStrike` | `calyrex_strike` | 打击 |
| `CatastrophicBlow` | `catastrophic_blow` | 大灾难 |
| `CleanUpFirst` | `clean_up_first` | 我先请？！ |
| `CourageRope` ⭐ | `courage_rope` | 勇气绳索 |
| `CrownTundra` | `crown_tundra` | 王冠雪原 |
| `DebugCard` ⭐ | `debug_card` | 调试 |
| `DarkGleam` | `dark_gleam` | 漆黑闪耀 |
| `DefendPosition` | `defend_position` | 固守 |
| `DivineBlessing` | `divine_blessing` | 神祝 |
| `DynamaxForm` | `dynamax_form` | 极巨形态 |
| `Encore` ⭐ | `encore` | 再来一次 |
| `ExtremeSpeed` | `extreme_speed` | 极速 |
| `FakeOut` | `fake_out` | 击掌奇袭 |
| `FriendlyGuard` | `friendly_guard` | 友情防守 |
| `Frost` | `frost` | 寒霜 |
| `FutureSight` | `future_sight` | 预知未来 |
| `Gallop` | `gallop` | 奔驰 |
| `GlacialWorld` | `glacial_world` | 冰封世界 |
| `GrassKnot` | `grass_knot` | 打草结 |
| `GrassyGlide` | `grassy_glide` | 青草滑梯 |
| `GrassyTerrain` | `grassy_terrain` | 青草场地 |
| `Harvest` | `harvest` | 收获 |
| `HelpingHand` | `helping_hand` | 帮助 |
| `HeroicSacrifice` | `heroic_sacrifice` | 英勇牺牲 |
| `HighHorsepower` | `high_horsepower` | 十万马力 |
| `HorseLove` | `horse_love` | 亲马爱 |
| `IcyWind` | `icy_wind` | 冰冻之风 |
| `Ingrain` | `ingrain` | 扎根 |
| `IntensivePlanting` | `intensive_planting` | 倾力种植 |
| `Intimidate` | `intimidate` | 威吓 |
| `LeechSeed` | `leech_seed` | 寄生种子 |
| `LonePath` | `lone_path` | 孤行 |
| `LongHowl` | `long_howl` | 长嚎 |
| `MaleficCurse` | `malefic_curse` | 诅咒 |
| `PPRestoreCard` | `pp_restore_card` | PP多项全补剂 |
| `PaleLance` | `pale_lance` | 苍白利矛 |
| `PartingShot` | `parting_shot` | 抛下狠话 |
| `PhantomForce` | `phantom_force` | 虚化 |
| `PlantCarrot` | `plant_carrot` | 种植萝卜 |
| `Pressure` | `pressure` | 压迫感 |
| `Protect` | `protect` | 守住 |
| `Psychic` | `psychic` | 精神强念 |
| `QuickAttack` | `quick_attack` | 电光火石 |
| `Recall` | `recall` | 回忆 |
| `RoyalFavor` | `royal_favor` | 王恩 |
| `SeedBomb` | `seed_bomb` | 种子炸弹 |
| `ShadowBall` | `shadow_ball` | 暗影球 |
| `SlowDown` | `slow_down` | 减速！！ |
| `SoulBlast` | `soul_blast` | 魂舞烈音爆 |
| `SoulHeart` | `soul_heart` | 魂心 |
| `SpikyDefense` | `spiky_defense` | 尖刺防守 |
| `StoredPower` | `stored_power` | 辅助力量 |
| `StrengthSap` | `strength_sap` | 吸取力量 |
| `Substitute` | `substitute` | 替身 |
| `TeraBlast` | `tera_blast` | 太晶爆发 |
| `Tribute` | `tribute` | 供养 |
| `Trick` | `trick` | 戏法 |
| `Truce` | `truce` | 休战 |
| `UproarRoar` | `uproar_roar` | 大声咆哮 |
| `WideGuard` | `wide_guard` | 广域防守 |
| `WishGrant` | `wish_grant` | 祈愿 |
| `ZenHeadbutt` | `zen_headbutt` | 意念头锤 |
| `CalyrexHaze` | `calyrex_haze` | 黑雾 |
| `IronDefense` | `iron_defense` | 铁壁 |
| `Fly` | `fly` | 飞翔 |
| `PollenPuff` | `pollen_puff` | 花粉团 |
| `Defiant` | `defiant` | 不服输 |
| `DracoMeteor` | `draco_meteor` | 流星群 |
| `LastResort` | `last_resort` | 珍藏 |
| `SandAttack` | `sand_attack` | 泼沙 |
| `CalyrexPounce` | `calyrex_pounce` | 扑击 |
| `EchoingWhinny` | `echoing_whinny` | 回响嘶鸣 |
| `ZCelebrate` | `z_celebrate` | Z-庆祝 |
| `PsychUp` | `psych_up` | 自我暗示 |
| `Spotlight` | `spotlight` | 聚光灯 |
| `MoveRecord` | `move_record` | 招式记录 |
| `IcicleCrash` | `icicle_crash` | 冰柱坠击 |
| `TripleAxel` | `triple_axel` | 三旋击 |
| `PsychicNoise` | `psychic_noise` | 精神噪音 |
| `Zero` | `zero` | 归零 |


### 卡面缺失清单（需补充素材）

以下 22 张卡暂无卡面（显示默认卡面），按 slug 命名放入 `assets/icons/card_portraits/`：

| slug | 中文名 | 类名 |
|---|---|---|
| `debug_card` | 调试 | DebugCard |
| `helping_hand` | 帮手 | HelpingHand |
| `protect` | 守住 | Protect |
| `psychic` | 精神强念 | Psychic |
| `substitute` | 替身 | Substitute |
| `tera_blast` | 太晶爆发 | TeraBlast |
| `wish_grant` | 祈愿 | WishGrant |
| `calyrex_haze` | 黑雾 | CalyrexHaze |
| `iron_defense` | 铁壁 | IronDefense |
| `fly` | 飞翔 | Fly |
| `pollen_puff` | 花粉团 | PollenPuff |
| `defiant` | 不服输 | Defiant |
| `draco_meteor` | 流星群 | DracoMeteor |
| `last_resort` | 珍藏 | LastResort |
| `sand_attack` | 泼沙 | SandAttack |
| `calyrex_pounce` | 扑击 | CalyrexPounce |
| `echoing_whinny` | 回响嘶鸣 | EchoingWhinny |
| `z_celebrate` | Z-庆祝 | ZCelebrate |
| `psych_up` | 自我暗示 | PsychUp |
| `spotlight` | 聚光灯 | Spotlight |
| `move_record` | 招式记录 | MoveRecord |
| `icicle_crash` | 冰柱坠击 | IcicleCrash |
| `triple_axel` | 三旋击 | TripleAxel |
| `psychic_noise` | 精神噪音 | PsychicNoise |
| `zero` | 归零 | Zero |
