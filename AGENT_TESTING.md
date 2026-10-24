# Agent 模组测试指南（蕾冠王 / 杀戮尖塔 2）

> 读者：接管本仓库、需要**真机验证**改动的 AI agent。
> 读完后应能独立跑完「改代码 → 构建 → 部署到测试机 → 驱动游戏验证 → 读日志下结论」的闭环。
> 机器坐标、账号、脚本细节在 `tools/testrig/README.md`（本机专用、已 gitignore，不入公开仓库）。

---

## 0. 一分钟上手

```powershell
$env:STS2_TEST_PW='<测试机密码>'
cd D:\vibeprograms\sts2CalyrexMod

pwsh -File tools\testrig\rig.ps1 start      # 建 SSH 隧道 + 确保游戏在跑
pwsh -File tools\testrig\sts2.ps1 health    # 期望：tunnel=ok, game=Hello from STS2 MCP, state=menu
pwsh -File tools\testrig\sts2.ps1 state     # 当前界面/战斗的完整 JSON
```

`health` 不是 `tunnel=ok` 就先排查隧道，别继续往下写测试结论。

---

## 1. 能力边界（先认清再设计测试）

| 能自动化验证 | 手段 |
|---|---|
| 菜单、角色选择、开局、读档 | `sts2.ps1 menu <option>` + `state` |
| 战斗：出牌、结束回合、用药、选择牌 | `play` / `end` / `potion` / `combatselect` |
| 卡牌数值、格挡、生命、能量、power 层数 | `state` 里的 `player` / `battle` |
| 敌人 intent、HP、状态 | `battle.enemies[*]` |
| 牌堆/弃牌堆/消耗堆、遗物、宠物、金币 | `player` 下各数组 |
| 地图、事件、商店、休息、宝箱、奖励 | 对应 `map` / `event` / `shop` / `rest` / `treasure` / `reward` |
| 卡池/遗物池规模、图鉴、存档 | `profile` / `compendium` / `profiles` |

| **不能**自动化验证 | 原因 / 替代 |
|---|---|
| 悬停提示框（HoverTip）、tooltip 内容 | 纯 UI，API 不暴露 → 人工看屏或截图 |
| 像素级视觉（卡面图、能量图标 NOPE、背景） | 同上 |
| 动画、音效、手感、鼠标拖拽 | 同上 |
| 截图 | 当前 RDP 会话无显示输出时全黑，见 §8 |

> 结论纪律：只通过 API 验证过的改动，交付时必须写「API 级验证通过，视觉/手感未实机确认」。

---

## 2. 测试台拓扑

```
本机 127.0.0.1:15526
   └─ ssh -N -L 15526:127.0.0.1:15526 <测试机>
        └─ SlayTheSpire2.exe（RDP 交互会话）
             ├─ mods\CalyrexMod\  ← 你的构建产物
             └─ mods\STS2_MCP.*   ← REST API（只监听 127.0.0.1，所以必须隧道）
```

- API 基址：`http://127.0.0.1:15526/api/v1/{singleplayer,multiplayer,profile,profiles,compendium,wiki}`。
- 前提：测试机有人登录着 RDP 会话（计划任务用 `/IT` 才能把游戏放到可见桌面）。会话掉了 `rig.ps1 status` 里 game 会变 `not running`。
- 游戏日志：测试机 `C:\Users\Administrator\AppData\Roaming\SlayTheSpire2\logs\godot.log`（`rig.ps1` 不封装日志读取，用 ssh 取，见 §7）。

---

## 3. 标准闭环

```powershell
# 1) 改代码：mod_src/**（卡牌/能力/patch）

# 2) 构建（隔离输出，别直接 dotnet build——csproj 的 CopyModToGame 会覆盖本地游戏 mods）
dotnet build CalyrexMod.sln -c Release -p:Sts2Dir=D:\vibeprograms\temp\sts2-build-check

# 3) 生成本地 pck + 本地部署（gen_tres / stage / make_pck）
python tools/deploy.py

# 4) 推送到测试机并重启游戏（会自动等 API 就绪）
pwsh -File tools\testrig\rig.ps1 deploy

# 5) 验证
pwsh -File tools\testrig\sts2.ps1 health
pwsh -File tools\testrig\sts2.ps1 state
```

- `rig.ps1 deploy` 会**先结束游戏进程**：dll/pck 被游戏占用时 scp 会失败（`dest open ... Failure`）。
- 只想推文件不重启：`rig.ps1 deploy -NoRestart`；只重启：`rig.ps1 restart`。
- 改完**只改了代码**也必须重推 pck 吗？是的——`deploy.py` 每次会重新 stage 资源并打包，dll 与 pck 需要配套。

---

## 4. 开一局蕾冠王

```powershell
# 主菜单（有进行中的 run 时）
pwsh -File tools\testrig\sts2.ps1 state          # 看 options
pwsh -File tools\testrig\sts2.ps1 menu abandon_run   # 放弃当前 run（只在菜单界面可用）

# 无 run 时主菜单会有 singleplayer
pwsh -File tools\testrig\sts2.ps1 menu singleplayer
pwsh -File tools\testrig\sts2.ps1 state          # menu_screen=character_select，characters[] 里有各角色

# 蕾冠王角色 ID 是 CALYREX_CHARACTER（对应 mod_src/Characters/CalyrexCharacter.cs）
pwsh -File tools\testrig\sts2.ps1 menu CALYREX_CHARACTER
pwsh -File tools\testrig\sts2.ps1 menu embark
```

- 角色锁定（`locked: true`）说明存档未解锁；测试机 profile3 已解锁过原版角色，蕾冠王由 mod 注册，正常应可选。
- `characters[]` 里 `starting_deck` / `starting_relics` / `total_cards` 可用于核对 mod 注册是否完整（卡池 95 张左右）。
- 开局后 `state_type` 依次是 `map` / `event` / `monster` …，`run.floor` 递增。

---

## 5. 状态机速查

| `state_type` | 含义 | 可用命令 |
|---|---|---|
| `menu` | 主菜单 / 角色选择 / 弹窗 | `menu <option>`；角色选择用 `menu <角色ID>` → `menu embark` |
| `overlay` | 覆盖层（如 FTUE） | 读 `message`/`options`，按需 `menu` |
| `monster` / `elite` / `boss` | 战斗 | `play <i> [-Target ID]`、`end`、`potion <slot>`、`combatselect`/`combatconfirm` |
| `hand_select` | 战斗中选牌（消耗/弃牌等） | `combatselect <i>` → `combatconfirm` |
| `card_reward` | 战后选牌 | `takecard <i>` / `skipcard` |
| `rewards` | 战利品结算 | `reward <i>`（从大索引往小拿）、`proceed` |
| `map` | 地图 | `map <i>`（`next_options` 里的 index） |
| `event` / `fake_merchant` | 事件 | `event <i>`、`dialogue`、`proceed` |
| `shop` | 商店 | `shop <i>`、`proceed` |
| `rest_site` | 营火 | `rest <i>`、`proceed` |
| `treasure` | 宝箱 | `treasure <i>`、`proceed` |
| `card_select` | 选牌界面（升级/移除/变化） | `select <i>` → `confirm` / `cancel` |
| `bundle_select` | 卡包预览 | `bundle <i>` → `bundleok` |
| `relic_select` | 选遗物 | `relic <i>` / `skiprelic` |
| `crystal_sphere` | 水晶球小游戏 | 用 `raw`：`{"action":"crystal_sphere_click_cell","x":..,"y":..}` |
| `game_over` | 结算 | `menu main_menu` |
| `unknown` | 过场/加载 | 等 2–3 秒重读，别急着判定失败 |

通用错误：**不在菜单界面时调 `menu` 会返回 `{"status":"error","message":"Not on a menu screen"}`**，属预期。

---

## 6. 战斗与索引纪律（最容易翻车的地方）

1. **出牌会让手牌索引左移**：要么从右往左打（大索引先），要么**每打一张就重读 `state`**。
2. 单体卡必须带 `-Target <entity_id>`（如 `SHRINKER_BEETLE_0`）；API 的错误信息会说明缺 target。
3. `can_play: false` 时看 `unplayable_reason`（能量不足 / HasUnplayableKeyword / 目标不合法…）。
4. `end` 之后敌我回合切换有延迟：**至少重读两次 `state`** 再判断（`turn` / `is_play_phase`）。
5. 数值验证用差分：出牌前后对比 `enemies[*].hp` / `player.block` / `player.energy`，不要只看描述文本。
6. 药水槽位是 `player.potions` 的下标，与手牌索引无关。

---

## 7. 本模组的验证套路

| 要验的东西 | 做法 |
|---|---|
| 卡牌数值/升级 | 开局读 `starting_deck`；战斗中出牌对照 HP/格挡差分；升级前后对比 `state` 里的 `description`（升级文本已内嵌） |
| 卡牌描述与能量图标 | API 只能读到 `description` 文本；图标/卡面必须人工看（§8） |
| 马匹系统 | 打出召唤马的牌 → `player.pets[]` 出现 `GLASTRIER`/`SPECTRIER`；喂养后看 `max_hp`；骑马合体后看玩家 powers/宠物消失 |
| **宠物马可被指定（P1）** | 有马在场时，用单体敌方卡（如花粉团）`-Target <马的 entity_id>`：修复前返回目标非法，修复后应成功并结算到马身上 |
| Boss「无极汰那」 | 需走到第 3 幕 Boss；单次测试成本高，优先用事件/种子或 dev console 跳过（`decompiled/` 与 `DevConsoleFix` 可参考），只做冒烟 |
| Patch 是否生效 | 看日志 `[CalyrexMod] ... patched=False/True` 行；Harmony 类若漏 `[HarmonyPatch]` 会静默跳过 |
| 注册是否完整 | `state` 的 `characters[]`（卡池数）、`profile`/`compendium`（解锁） |

日志读取（示例）：

```powershell
$env:SSH_PW='<密码>'; $env:SSH_ASKPASS='<rig 生成的 askpass>'; $env:SSH_ASKPASS_REQUIRE='force'
ssh Administrator@<测试机> "powershell -NoProfile -Command `"Select-String -Path $env:APPDATA\SlayTheSpire2\logs\godot.log -Pattern 'CalyrexMod|ERROR|Exception' | Select-Object -Last 30`""
```

关键 grep：`[CalyrexMod]`、`Finished mod initialization`、`DuplicateModel`、`Harmony`、`Exception`、`Asset not cached`。

---

## 8. 截图（当前受限）

- 现状：RDP 会话没有活动显示输出时，`CopyFromScreen` 与 `PrintWindow(PW_RENDERFULLCONTENT)` 都只能得到 **1920×1080 纯黑**（实测 centerPixel=0,0,0）。
- `rig.ps1 shot` 仍然会把图拉到 `tools/testrig/shots/`，但多半是黑图。
- 需要真实画面时：
  1. 让测试机的 RDP 桌面处于**真正连接并被查看**的状态（人或 mstsc 挂着），再 `rig.ps1 shot`；
  2. 或在**本地机**跑同一份构建做人工目视（本地 mods 目录也是同一版本）。
- 替代证据：用 `state` 里的文本字段（`description`、`keywords`、`powers`）做内容断言，截图只用于视觉项。

---

## 9. 排障速查

| 现象 | 处理 |
|---|---|
| `health` 显示 tunnel fail | `rig.ps1 start`；本地 15526 被占则先 `rig.ps1 stop` |
| 游戏在跑但 API 不通 | 看日志有没有 `[STS2 MCP] v0.4.0 server started`；确认 `mods\STS2_MCP.conf` 端口 15526 |
| 游戏起不来 | `schtasks /Query /TN STS2_Test_Run /V` 看 Last Result；确认 RDP 会话 Active（`qwinsta`） |
| `Steamworks initialization failed: No appID` | 游戏目录缺 `steam_appid.txt`（内容 `2868840`） |
| deploy 报 `dest open ... Failure` | 游戏占着 dll/pck —— `rig.ps1 deploy` 已内置先停进程；手工 scp 前先 `rig.ps1 stop` |
| 日志出现 `Disabling the Steam workshop version` | 正常：测试机订了工坊版，加载器保留本地 mods 版本 |
| `DuplicateModelException` | 本地版与工坊版/exp 项目同时加载；删一份或禁用 |
| `RitsuLib ... MissingMethodException` | 测试机其他 mod 的版本冲突，与 CalyrexMod 无关 |

---

## 10. 安全与边界

- **只动测试机存档（profile3）**；不要在本机正式存档上跑自动化。
- 密码只从 `$env:STS2_TEST_PW` 传入，**不要写进脚本/仓库**。
- `tools/testrig/` 已 gitignore（含内网地址、截图、askpass 临时文件）；不要把测试机信息提交到公开仓库。
- 一次只跑一个游戏进程（dll 锁 + 端口冲突）；跑完不一定要关，但改代码重部署前必须停。
- 测试结束前 `git status` 确认没有意外产物（`build/`、`bin/`、`obj/` 都在 ignore 里）。

---

## 11. 命令速查

```powershell
# 生命周期
rig.ps1 status | start | stop | restart | deploy [-NoRestart] | shot

# 游戏
sts2.ps1 health
sts2.ps1 state [-Format json|markdown]
sts2.ps1 menu <option>            # continue / abandon_run / singleplayer / CALYREX_CHARACTER / embark / quit
sts2.ps1 play <i> [-Target ID]
sts2.ps1 end
sts2.ps1 potion <slot> [-Target ID]
sts2.ps1 discard <slot>
sts2.ps1 map <i>
sts2.ps1 rest <i>
sts2.ps1 event <i>
sts2.ps1 dialogue
sts2.ps1 proceed
sts2.ps1 reward <i>
sts2.ps1 takecard <i> | skipcard
sts2.ps1 select <i> | confirm | cancel
sts2.ps1 combatselect <i> | combatconfirm
sts2.ps1 bundle <i> | bundleok
sts2.ps1 relic <i> | skiprelic
sts2.ps1 treasure <i>
sts2.ps1 shop <i>
sts2.ps1 profiles | profile | compendium | wiki <query>
sts2.ps1 raw POST /api/v1/singleplayer '{"action":"end_turn"}'

# 底层 REST（等价）
Invoke-RestMethod http://127.0.0.1:15526/api/v1/singleplayer?format=json
```

---

## 12. 可选：接成 MCP 工具（需重启 DSH）

`tools/testrig/mcp/` 是 upstream MCP 桥（MIT，64 个工具，含 `get_game_state`、`combat_play_card`、`menu_select`…）。
自检：

```powershell
$env:UV_CACHE_DIR='D:\vibeprograms\mcp-servers\uv-cache'
uv run --directory tools\testrig\mcp python _selftest.py    # 期望 tools=64
```

注册进 DSH 需要在 `~/.dsh/profiles/desktop/cordis.patch.yml` 加一条 stdio 配置并**重启 DSH**，配置样例见 `tools/testrig/README.md` §MCP。
不接 MCP 也完全可用——`sts2.ps1` 是同一套 REST 的薄封装。

---

## 13. 一页流程模板（复制改改就能用）

```powershell
$env:STS2_TEST_PW='…'
cd D:\vibeprograms\sts2CalyrexMod
pwsh -File tools\testrig\rig.ps1 start
# 1) 验证改动前先记录基线
pwsh -File tools\testrig\sts2.ps1 menu continue       # 或 singleplayer → CALYREX_CHARACTER → embark
pwsh -File tools\testrig\sts2.ps1 state > before.json
# 2) 操作
pwsh -File tools\testrig\sts2.ps1 play 3 -Target <ID>
pwsh -File tools\testrig\sts2.ps1 end
Start-Sleep 2; pwsh -File tools\testrig\sts2.ps1 state > after.json
# 3) 对比 before/after 的 hp/block/energy/powers，再决定是否继续跑
# 4) 复盘：日志 grep [CalyrexMod]，把「API 验证过 / 未验证」写清楚
```
