# sts2CalyrexMod 蕾冠王模组

《杀戮尖塔 2》的蕾冠王主题模组。用 0Harmony 原生 API 开发，无第三方游戏库依赖。

> **[Steam 创意工坊](https://steamcommunity.com/sharedfiles/filedetails/?id=3790420158) 可直接订阅游玩**（本仓库为源码版）。

> 先说明白：图片资源还没做完。卡面大部分还是占位图，一些图标和背景是临时凑的，后面会慢慢补。内容本身是完整的，能从头玩到尾。

## 功能

- **新角色**：蕾冠王（Calyrex），独立卡池 80+ 张卡（含先古牌），专属能量图标、卡框、地图颜色
- **双马系统**：黑白萝卜召唤雪暴马/灵幽马，喂养提升生命，骑马合体得专属力量；马匹守护自动承受伤害（白马先扛，死了溢出给黑马，马死后喂养可复活）
- **事件**：雪拉比快递、宝可梦寄养屋；8 个原版先古事件新增蕾冠王专属对话
- **宝可梦联赛**：炽焰咆哮虎 → 烈咬陆鲨 → 超坏星 → 巨锻匠/谜拟丘轮换，专属遗物（王者之证、特性膏药、怯场）
- **Boss**：无极汰那（Eternatus），两阶段战斗（死亡复活 + 无极巨化），阶段专属 BGM 无限循环；加入荣耀幕 Boss 池随机出现（不替换原 Boss）
- **其他**：30+ 遗物、5 种药水、30+ 技能修正（重放、骑马、学习装置等）、中英双语本地化

## 构建

需要游戏本体（`Slay the Spire 2`），依赖 dll 从游戏目录复制：

```powershell
# 1. 复制游戏依赖（路径按实际安装位置改）
$game = "D:\SteamLibrary\steamapps\common\Slay the Spire 2"
Copy-Item "$game\data_sts2_windows_x86_64\*.dll" libs\
Copy-Item "$game\data_sts2_windows_x86_64\managed\*.dll" libs\

# 2. 生成 .tres（从 png 批量生成，不入库）
python tools/gen_tres.py

# 3. 编译 + 打包 + 部署到 mods 目录
python tools/deploy.py
```

## 目录结构

```
mod_src/            源码（Cards / Powers / Relics / Monsters / Events / Patching / Audio / Characters）
assets/             本地化（eng/zhs）、图标、怪物视觉、BGM、场景
tools/              deploy.py（一键构建部署）、gen_tres.py（png→tres）、make_pck.py（pck 打包）
docs/               开发文档
game/               游戏解包（不提交，用于参照官方实现）
decompiled/         sts2 反编译工程（不提交）
```

## 技术要点

- **卡牌 slug 规则**：本地化 key 用大写蛇形（连续大写也拆，`PPRestoreCard` → `P_P_RESTORE_CARD`）；卡面 slug 是 snake_case 且连续大写合并（`pp_restore_card`），尺寸 1000×760
- **图片加载**：裸 png 无 `.import` 无法被 ResourceLoader 加载，一律用 `.tres` 内嵌（`tools/gen_tres.py` 生成）；BGM 用裸 PCM 字节构造 `AudioStreamWav` 绕开 `.import`
- **代码规范**：类名避免与内置类冲突（撞过 Uproar/Catastrophe/Wish/Feed/Haze/Pounce，统一加前缀）；升级用 `UpgradeValueBy()`；Harmony patch 全 try-catch
- 详细开发规范见 `docs/开发文档.md`

## 版本

- v1.1：所有怪物动作填充动画键名；无极汰那改为随机加入荣耀幕 Boss 池；马匹守护、喂养复活、Boss 胜利结算等修复

## 致谢

游戏：《Slay the Spire 2》MegaCrit。宝可梦内容版权归任天堂/Game Freak 所有，本模组为粉丝自制，非商业用途。

## 版本更新说明

每个版本的独立详细更新说明见 `docs/releases/`（从 v0.9 到当前全部版本），汇总见 `CHANGELOG.md`。
