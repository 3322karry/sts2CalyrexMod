# AGENTS.md — CalyrexMod 项目记忆与接手指南

> 本文件是给 AI agent（及人类协作者）的完整项目记忆。新会话接手时先通读本文件。
> 最后更新：2026-09-25（v1.2.42.107.1 发布后）

---

## 1. 项目概览

- **是什么**：《杀戮尖塔 2》(Slay the Spire 2) 的蕾冠王（宝可梦）主题 Mod。新增角色「蕾冠王」、88+ 卡牌、14 遗物、5 药水、专属事件、先古对话、新 Boss「无极汰那」。
- **技术栈**：C# / .NET 9 / Godot 4.5.1 / Harmony (0Harmony) / sts2.dll 原生 API。
- **作者**：LampyStar_灯星兰（所有署名统一此名）。
- **游戏版本**：v0.107.1（版本号后缀 `107.1` 即游戏版本）。
- **发布渠道**：
  - GitHub: `https://github.com/3322karry/sts2CalyrexMod`（主仓库 + Wiki 仓库 `sts2CalyrexMod.wiki.git`）
  - Steam 工坊: item `3790420158`（账号 `Lampy_LanPii`，workspace title「蕾冠王模组-CalyrexMod」）
- **当前版本**：`v1.2.42.107.1`（已发布，Steam/GitHub 同步）。

### 目录与路径（本机）

| 用途 | 路径 |
|---|---|
| 主项目 | `D:\vibeprograms\sts2CalyrexMod` |
| RitsuLib 实验项目（本地 git，不发布） | `D:\vibeprograms\CalyrexMod-exp` |
| Wiki 本地克隆 | `D:\vibeprograms\sts2CalyrexMod-wiki` |
| Steam 上传器（用户提供） | `D:\vibeprograms\ModUploader-win-x64`（workspace 子目录 `sts2CalyrexMod`） |
| 游戏目录 | `D:\SteamLibrary\steamapps\common\Slay the Spire 2` |
| 游戏 mods | `...\Slay the Spire 2\mods\CalyrexMod`（部署目标） |
| 游戏日志 | `C:\Users\yy150\AppData\Roaming\SlayTheSpire2\logs\godot.log` |
| 反编译游戏源码 | 项目内 `decompiled/`（参考用，不参与编译）；`src/` 是游戏源码 junction（仅编辑器） |
| 依赖 DLL | 项目 `libs/`（sts2.dll, 0Harmony.dll, GodotSharp.dll, SmartFormat.dll，不入库） |

---

## 2. 项目结构要点

- **只编译 `mod_src/`**（csproj 里 `EnableDefaultCompileItems=false` + 显式 Include）。
- **`assets/`**：全部 mod 资源（本地化 JSON、图标、场景、音频）。
  - `assets/localization/{zhs,eng}/`：cards/powers/relics/potions/events/ancients 等 JSON。
  - **所有 PNG 必须经 `tools/gen_tres.py` 转成 `.tres`（内嵌 RGBA PackedByteArray）**，因为 mod 的 pck 无法带 `.import`，Godot 无法直接加载裸 png。
- **`CalyrexMod.json`**（项目根）：mod 清单（id/name/version/author/dependencies）——**部署与上传都用这个文件**。
- **`mod_src/ModInfo.cs`**：`ModId/ModName/Version` 常量。
- **`mod_src/ModEntry.cs`**：`[ModInitializer]` 入口——Harmony PatchAll + 卡池注册（`ModHelper.AddModelToPool<Pool, Card>()`）+ SavedProperties 注入。
- **`docs/releases/`**：每版本独立发布页（`vX.Y.Z.107.1.md`）。
- **`CHANGELOG.md`**：版本更新日志（顶部最新）。
- **`tools/`**：全部构建/发布工具（见 §5）。

---

## 3. 核心机制与关键知识（踩坑总结）

### 3.1 能量图标系统（三代机制，易踩坑）

1. **卡面费用图标**：`CardPoolModel.EnergyIconPath` getter 被 patch（`CalyrexVisualPatches`），CalyrexCardPool → `res://CalyrexMod/icons/energy_calyrex.tres`。
   - **该资源必须与官方同构：`AtlasTexture`（内嵌 64×64 ImageTexture + region）**——曾经是 985KB 的 ImageTexture（256×256），在部分设备加载失败 → 游戏显示 Godot 缺失纹理占位 **"NOPE"**。
   - 官方格式参考（游戏 pck 内 `images/atlases/ui_atlas.sprites/card/energy_ironclad.tres`）：`[gd_resource type="AtlasTexture"] + atlas + region = Rect2(...)`。
2. **描述中的图标**：`EnergyIconFormatterPatch`（Harmony Prefix on `EnergyIconsFormatter.TryEvaluateFormat`）。
   - **只有当图标前缀为 `calyrex` 时才替换为 `.tres`**，其他角色/无色卡走官方（原版 `*_energy_icon.png`）。
   - 前缀解析顺序：`EnergyVar.ColorPrefix` → **字符串值本身**（`{energyPrefix:...}` 变量，卡牌库等无 run 场景的关键）→ `RunManager.Instance.GetLocalCharacterEnergyIconPrefix()`。
   - 修过一次重大 bug：原来无条件替换所有能量图标 → 其他角色卡面全变蕾冠王图标。
3. **游戏路径欺骗**：`deploy.py` 会额外把资源 stage 到游戏原路径（pck 内）：
   - `images/atlases/ui_atlas.sprites/card/energy_calyrex.tres`
   - `images/packed/sprite_fonts/{calyrex,colorless}_energy_icon.png`
   - 卡框材质、药水图集同理。

### 3.2 卡牌描述官方格式（v1.2.42 对齐）

- 数值差异：`{Damage:diff()}`（升级预览/升级后自动绿色显示 9→12）。
- 升级文字变化：`{IfUpgraded:show:<升级后文本>|<未升级文本>}`（如 `[gold]{IfUpgraded:show:归零+|归零}[/gold]`）。
- 能量图标：`{Energy:energyIcons()}`（EnergyVar，渲染"1+图标"）；`{energyPrefix:energyIcons(N)}`（N 个能量图标）；"0 费"写作 `0{energyPrefix:energyIcons(1)}`。
- 关键词颜色：原版关键词用 `[gold]`（易伤/力量/敏捷/消耗/保留…）；mod 关键词沿用 `[green]`（丰饶）/`[blue]`（冰冻）。
- **描述里的变量由 `CardModel.GetDescriptionForPile` 注入**（`energyPrefix`、`IfUpgraded`、`OnTable`、`InCombat` 等）。
- 卡池 EnergyColorName = `"calyrex"`；Potion/Relic 池 = `ironclad`。
- **改动描述后必须重新 `deploy`（本地化在 pck 里）**。

### 3.3 提示框（HoverTips）机制

- **描述里出现的每个名词都应挂对应 ExtraHoverTips**（官方连原版关键词也显式挂，如 Bash 挂 Vulnerable）。
- 工具（`mod_src/Cards/KeywordTipHelper.cs`）：
  - 关键词 → `HoverTipFactory.FromKeyword(CardKeyword.Xxx)`（消耗/虚无/保留/固有）
  - power → `HoverTipFactory.FromPower<XxxPower>()`（力量/敏捷/易伤/脆弱/虚弱/丰饶/冰冻…）
  - 卡牌 → `HoverTipFactory.FromCard<Xxx>()`（归零/星碎/雪矛）
  - mod 关键词 → `KeywordTipHelper.{MountTip,FeedTip,AbundanceTip,FrozenTip,...}`
- v1.2.42 已用盘点脚本给 42 张缺失的卡补全（脚本思路：读 zhs cards.json 描述匹配名词集合 vs 源码里的 tip 集合，输出差集）。

### 3.4 马匹系统（Steed）

- 双马：Glastrier（白马/雪暴马）、Spectrier（黑马/灵幽马）。`MountHelper` 统一：
  - `SpawnSteed(owner, type)`：AddPet + `SteedTargetablePower`（可被打/死亡留场）。
  - `DoMount(ctx, owner, source, preselectedSteed = null)`：preselected 有值时跳过选择直接合体（奔驰卡用）。
  - `DoUnmount`：解除合体，恢复马 MaxHp。
  - `FeedBoth/FeedOne`：喂养（战斗开始马为 0 hp，喂养给 MaxHp；死马 GainMaxHp 复活并补挂 **迅捷之视 (QuickSight)/重装之矛 (HeavyLance)**）。
- **洗入星碎/雪矛的 QuickSight/HeavyLance 挂在玩家身上**（挂马不可靠），DoMount 从玩家读取层数。
- **`SteedTargetablePower.ShouldCreatureBeRemovedFromCombatAfterDeath` 对非自己必须返回 true**（v1.2.20 关键 bug：曾误拦所有敌人死亡移除）。
- 骑马合体：`MountedGlastrier/MountedSpectrier`（Amount = 马 MaxHp）+ `EternalWhinny` + `MountMergePower`（防死亡触发）+ 视觉替换 + 单敌战斗合体给双 debuff。

### 3.5 先古对话（Ancient Dialogue）

- `CalyrexAncientDialoguePatch`：注入 `AncientDialogue` 必须设 `VisitIndex`（`GetValidDialogues` 按 `VisitIndex == charVisits` 过滤）且重复组 `IsRepeating = true`（`AddRepeatingDialogues` 要求）。
- 文本填充**直接构造 LocString**（避免注入早于 mod 表合并选错 key）。
- 单组注入（涅奥 3 行/其他 1 行，VisitIndex=0+IsRepeating）——任意访问显示同段。
- **`AncientBannerFix`**：Prefix 跳过 `NAncientNameBanner.AnimateVfx`，规避 StS2ZhFont/BaseLib 的 banner patch 崩溃（ObjectDisposedException 导致部分先古事件 UI 中断）。

### 3.6 其他关键 patch

- `CalyrexCharacter`：`IconPath/IconOutlineTexture/EnergyCounterPath/MerchantAnimPath/RestSiteAnimPath/CharacterSelectBg/Transition` 全部按角色判断重定向。
- **模型注册**：卡必须 `ModHelper.AddModelToPool<Pool, Card>()`（在 ModEntry.Stage2Register）；角色/遗物/药水靠 ModelDb 扫描 + `SavedPropertiesTypeCache.InjectTypeIntoCache`（新遗物要加）。
- **涅奥大胶囊**（LargeCapsule）：`GetStrikeForCharacter/GetDefendForCharacter` 对 CalyrexCharacter 返回专属打击/防御（否则卡死）。
- **无极汰那（Eternatus）Boss**：两阶段 + 复活状态机 + 专属 BGM（AudioStreamWav 内嵌 PCM）+ 背景覆盖 + 地图图标 + 随机 Boss 池（追加不替换）；Boss 战后 Epoch 检查 patch 跳过 mod 角色。
- **`AssetCache.GetAsset` 泛型重载歧义会炸 PatchAll**——统一用 `GetTexture2D`/`GetCompressedTexture2D` 层重定向；类级 `[HarmonyPatch]` 标注必须齐全（否则 PatchAll 跳过整个类）。
- 资源缺失重定向（run_history/eternatus_boss.png → queen_boss 等）在 `AssetIconPatches`。

### 3.7 卡牌代码约定

- `CanonicalVars` 提供 DynamicVar（数值一定要走变量，方便升级 diff 与描述引用）。
- 升级逻辑放 `OnUpgrade()`（数值 `UpgradeValueBy`）；**不要写 `IsUpgraded ? A : B` 硬编码**。
- 消耗/虚无等关键词用 `CanonicalKeywords => new[] { CardKeyword.Exhaust }`。
- 卡描述 key = 类名转 UPPER_SNAKE（`CalyrexHaze` → `CALYREX_HAZE`）。

---

## 4. 版本号规则

- **mod 版本**：`v1.2.x.107.1`（原格式；107.1=游戏版本）。游戏内/清单/Steam/docs 一致。
- **exp 项目**：`v1.3.00.107.1-exp`。
- **Wiki 更新记录**：`v0.3.YYYY.MMDD.NN.模组版本`（NN 全局递增，当前到 12）。
- 每次改 mod 内容 → 升版本（release.py 自动处理 ModInfo.cs + CalyrexMod.json）。

---

## 5. 工具链（tools/）

### 5.1 deploy.py（构建部署）

`python tools/deploy.py` → 流程：`gen_tres.py` → stage assets 到 `build/pck_root/CalyrexMod/`（模拟游戏 res:// 路径）→ `make_pck.py` 打包 `build/CalyrexMod.pck` → 复制 **dll（bin/Release/net9.0）+ pck + 根 json** 到游戏 mods 目录。
- **pck 内部路径一律 `res://CalyrexMod/...`**（代码资源引用不变；与 mod 目录名无关）。
- csproj 的 `CopyModToGame`/`ExportPck` target 存在（build 后自动复制 dll+json，但 pck 走 deploy.py）。

### 5.2 gen_tres.py（png→tres）

- 遍历 `assets/` 下 png → 生成同名 `.tres`（ImageTexture + 内嵌 PackedByteArray 十进制 RGBA8）。
- **特判 `energy_calyrex`：生成 AtlasTexture 形态（64×64）+ region**（官方同构，防 NOPE）。
- png mtime 新于 tres 才重新生成（删 tres 可强制重建）。

### 5.3 release.py（一键发版）★核心工具

```
python -X utf8 tools/release.py v1.2.43.107.1 --note "改动1" --note "改动2" \
  [--steam-note "English changeNote"] [--wiki-summary "简短语"] \
  [--skip-steam|--skip-wiki|--skip-deploy|--skip-push|--dry-run]
```

10 步：前置检查（关游戏）→ 版本号（幂等）→ CHANGELOG → docs/releases 页 → Wiki 同步（版本历史+版本列表，序号自动 +1）→ 构建 → 部署 → git 推送（主仓库）→ git 推送（Wiki）→ Steam（content 同步 + changeNote + 上传，k_EResultFail 自动重启 Steam 重试一次）。

**关键实现细节（踩坑）**：
- **Steam content 同步源**：dll=`bin/Release/net9.0/`，json=项目根，pck=`build/`。**曾经错用 `build/` 的旧 dll/json（v0.9.0）上传——这是"工坊显示旧版"的元凶**，已修复。
- `build/` 目录里的旧 `CalyrexMod.dll/json` 已删除，勿再放入。
- Steam 上传 `k_EItemUpdateStatusInvalid` 是**常态**（提交状态枚举，内容通常已上传成功）——只要出现 "Successfully uploaded" 即可。
- `k_EResultFail` = Steam 上传会话损坏 → **重启 Steam 后第一次上传必成功**（脚本已自动处理）。

### 5.4 手动上传（应急）

```
cd D:/vibeprograms/ModUploader-win-x64
./ModUploader.exe upload -w "D:/vibeprograms/ModUploader-win-x64/sts2CalyrexMod"
```
- 上传前确认 `sts2CalyrexMod/content/` 三件套是最新（dll 407KB 级别、json 版本号、pck ~270MB）。
- workshop.json 的 `description` 用**英文**（用户要求）；`changeNote` 英文。

---

## 6. Steam 工坊配置（workshop.json）

- title：`蕾冠王模组-CalyrexMod`；visibility：public；tags：Characters/Cards/Audio/Monsters/Events/Relics/Potions/Bosses；dependencies：[]
- description：**英文**（Calyrex Mod 简介 + Wiki 链接 + "Requires BaseLib" + CC BY-NC 4.0）。
- changeNote：英文。

---

## 7. RitsuLib 实验项目（CalyrexMod-exp）

- 位置 `D:\vibeprograms\CalyrexMod-exp`（本地 git，**不发布**，仅实验）。
- 迁移要点：
  - `libs/ritsu/`（RitsuLib v0.6.2 compat/0.107.1 + shared），csproj `Import RitsuLib.References.props`。
  - **ModEntry 注册改为**：`RitsuLibFramework.CreateContentPack(ModInfo.ModId).Card<Pool, Card>()...Apply()`（86 个注册）。
  - AssemblyName/id = `CalyrexMod-exp`，`dependencies: ["STS2-RitsuLib"]`；pck 内部 res:// 路径保持 `CalyrexMod`。
  - **csproj 自动部署已禁用**（`DeployToGame` 开关，默认不部署；需要时 `python tools/deploy.py`）。
- **RitsuLib 游戏内安装**（如需）：完整解压到 `mods/STS2-RitsuLib/`（必须含 `compat/<ver>/`、`shared/`、`assets.zip`、`mod_manifest.json`，并复制一份 `STS2-RitsuLib.json`）——**只放根 dll 会报找不到 Runtime**。
- **实验注意**：exp 与工坊/本地 CalyrexMod **同时加载会 DuplicateModelException**（同名模型类）——测试时禁其一。

---

## 8. 已知问题与注意事项

1. **Steam 上传**：`Invalid` 常态可忽略；`k_EResultFail` 重启 Steam 解决；**content 文件源必须正确**（见 §5.3）。
2. **能量图标 NOPE**：= Godot 缺失纹理占位。检查 `energy_calyrex.tres`（必须 AtlasTexture/64×64）与 patch 前缀逻辑。
3. **描述/图标改动后必须重新 deploy**（本地化与资源在 pck 内）。
4. **`ModHelper.AddModelToPool` 对每个新卡必须调用**，否则卡不会出现。
5. **Wiki 本地**在 `D:\vibeprograms\sts2CalyrexMod-wiki`（**勿放 Temp——会被清理**）；release.py 已按此路径配置。
6. **双 mod 冲突**：任何同名模型类都不允许两个程序集同时加载。
7. **游戏启动崩溃排查**：读 `godot.log` 搜 `[CalyrexMod]`、`Exception`、`DuplicateModel`、`Asset not cached`。
8. **反编译参考**：`decompiled/` 目录（关键类：`EnergyIconsFormatter`、`EnergyIconHelper`、`CardModel`（GetDescriptionForPile/ExtraHoverTips）、`CardPoolModel`、`HoverTipFactory`、`CardCmd`（TransformTo/Upgrade）、`ShowIfUpgradedFormatter`、`PreloadManager`）。
9. **SmartFormat 变量**：描述里 `{X}` 来自 DynamicVars + `GetDescriptionForPile` 注入的变量（energyPrefix/IfUpgraded/OnTable/InCombat…），formatter 有 diff/energyIcons/starIcons/show(IfUpgraded)/choose 等。
10. **发布后同步**：GitHub 主仓库 + Wiki 都要推（release.py 自动），Steam 上传后用户侧刷新工坊验证。

---

## 9. 给下一个 Agent 的快速上手

### 常见任务流程

**A. 修改卡牌数值/效果**
1. 改 `mod_src/Cards/Xxx.cs`（DynamicVar + OnUpgrade，勿硬编码 IsUpgraded）。
2. 若描述需要改：`assets/localization/zhs|eng/cards.json`（官方格式，见 §3.2）。
3. 名词提示：确认 `ExtraHoverTips` 覆盖描述里所有名词（§3.3）。
4. `dotnet build -c Release` → `python tools/deploy.py` → 启动游戏测试。

**B. 发布新版本**
```
python -X utf8 tools/release.py v1.2.43.107.1 --note "改动" --steam-note "English note"
```
（先确认游戏已关；脚本自己处理 git/Wiki/Steam。）

**C. 调试**
- 日志 `%appdata%\SlayTheSpire2\logs\godot.log`；临时诊断可加 `Log.Info("[CalyrexMod] ...")`（发布前清理）。
- 悬停/描述问题 → decompiled 找 formatter；资源加载失败 → 检查 pck 是否含该路径（可用 python 搜 pck 字节）。

### 绝对不要做

- 不要用 `build/CalyrexMod.dll|json` 当上传源（旧产物，已删）。
- 不要把 mod pck 内资源路径从 `res://CalyrexMod/` 改掉。
- 不要给 `SteedTargetablePower.ShouldCreatureBeRemovedFromCombatAfterDeath` 对非自己返回 false。
- 不要无条件替换全局能量图标（必须按 prefix 判断）。
- 不要在没有隔离（禁用另一 mod）时同时加载 exp 与正式 CalyrexMod。
- 发布前确保 `git status` 的改动都是想发布的（release.py 会全量提交）。
