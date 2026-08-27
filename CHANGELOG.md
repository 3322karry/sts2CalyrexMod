# Changelog / 更新日志

## v1.2.10.107.1 (2026-08-27)

### Changes / 改动
- **雪拉比快递 / 宝可梦寄养屋**：遗物选项保留，但遗物随机池**排除联赛专属遗物**（王者之证、特性膏药）——这两个遗物只在宝可梦联赛获得

## v1.2.09.107.1 (2026-08-27)


### Changes / 改动
- **雪拉比快递**：移除遗物选项（不再给予遗物，保留药水/金币选项）
- **宝可梦寄养屋**：移除钱袋选项（不再花费 100 金币换取遗物，保留蛋/进化石选项）

## v1.2.08.107.1 (2026-08-27)


### Fixes / 修复
- **雪拉比快递 / 宝可梦寄养屋拿不到遗物**：旧 RelicIconPatches 硬编码连字符图标路径与重命名后的下划线文件不匹配，导致遗物图标加载失败、获取流程中断——已删除旧 patch，统一由 AssetIconPatches（动态路径）接管

## v1.2.07.107.1 (2026-08-27)


### Fixes / 修复
- **花粉团**：改为对敌人造成 9/12 伤害时，两匹马同时回复 33%/50% 最大生命（目标为马时直接回血该马）——不再依赖目标界面能否选中宠物

## v1.2.06.107.1 (2026-08-27)


### Fixes / 修复
- **全力一击等精英战胜利卡死**：CheckFifteenElitesDefeatedEpoch 对 mod 角色抛异常（同 Boss 版），已跳过

### Changes / 改动
- **聚光灯**：重做为指定一名敌人——只要它存活，"对所有敌人的伤害"只命中它，且它受到的这种伤害 +50%（升级 +100%）

## v1.2.05.107.1 (2026-08-27)


### Changes / 改动
- **花粉团**：可指定敌人或马——对敌人造成 9/12 点伤害；对马回复其 33%/50% 最大生命
- **爆音波**：改为对所有单位（敌人 + 两匹马）造成 {Damage} 伤害，自身受到的伤害减少 25%（升级后 50%），不再有固定 7 血自伤
- **机制**：宠物马现在可作为卡牌指定/攻击目标（HittableEnemies 补丁，与马匹守护可被打设计一致）

## v1.2.04.107.1 (2026-08-27)


### Polish / 优化
- **文本清晰化**（中英同步）：
  - 卡牌：13 张描述优化（全力一击/爆音波/黑雾/极巨形态/回响嘶鸣/守住/自我暗示/减速/戏法/祈愿/种萝卜/吸取力量 等，语义更准确）
  - 能力：7 条描述修正（青草场地层数 3 与卡一致、减速语句、寄生种子回复对象、加速标点、永恒嘶鸣、丰饶措辞、精神噪音）
  - 删除重复的 PP 多项全补剂旧 key（P_P_RESTORE_POWER）
- **词条提示**：种萝卜补充迅疾之视/重装之矛提示

## v1.2.03.107.1 (2026-08-27)


### Fixes / 修复
- **升级提示**：修正为游戏原生绿色 diff 机制（`{X:diff()}`）——16 张卡补齐升级后数值变量（焦急/牵绊缰绳/勇气绳索/神恩/疾驰/雪矛/寒风/密集种植/长嚎/花粉团/减速/聚光灯/积蓄力量/替身/太晶爆发/Z-庆祝 等），升级后变化数值绿色显示；移除直接写在卡面上的升级文本
- **词条提示**：黑雾（CalyrexHaze）补充"归零"卡提示框（FromCard<Zero>）

### New / 新增
- 骑马选择（骑白马/骑黑马/下马）、归零（丰饶）、喂养类等 11 张卡 hover tips（自 v1.2.1.02 起）

## v1.2.1.02 (2026-08-27)


### New / 新增
- **升级后描述**：29 张卡补充升级说明（费用变化/固有/保留/不再消耗/自伤变化等，中英）
- **词条提示**：补齐骑马选择（骑白马/骑黑马/下马）、归零（丰饶）、喂养类（马匹之爱/种子炸弹/奉献/Z-庆祝/草场）等 11 张卡的 hover tips

## v1.2.1hotfix (2026-08-27)


### Fixes / 修复
- **骑马喂养**: 喂养不再复活合体中的马（骑黑马时只复活/喂养在场白马，不会把黑马重新召唤），骑马选择不再出现两匹马 / Feeding no longer revives the mounted (merged) steed; only the alive steed is fed/revived, so mounting shows only the correct steed.

# Changelog / 更新日志

## v1.2 (2026-08-27)

### Fixes / 修复
- **Assets display**: card portraits / relic icons now load correctly (new AssetIconPatches redirecting path getters to mod resources); .jpg portraits supported; wrong filenames fixed (phychic→psychic, with_grant→wish_grant, pp_restore_card→p_p_restore_card, kings-rock→king_rock, etc.)
- **PP多项全补剂 (PP Restore) portrait**: 160×160 image was too small to display, now processed
- **Card library lag**: all portraits/events/backgrounds compressed (decimal .tres total 933MB→219MB, pck 1.4GB→177MB)
- **Freeze after defeating boss**: mod character has no registered Epochs, now skips ObtainCharUnlockEpoch / CheckFifteenBossesDefeatedEpoch
- **Dynamax Form description**: "until the end of next turn" → "for 3 turns" (ZH/EN)
- **Eternatus stuck after revive**: dead state machine now has FollowUpState branch (phase1/phase2), all P2 states registered in the state machine list

### New / 新增
- **Dynamax Form visuals**: character sprite scales up 3x when played, resets when it ends
- **Eternatus boss fight**: custom background overlay, phase 2 Eternamax sprite swap + 3x scale
- **Eternatus appears randomly**: added to the Glory act boss pool (no longer replaces the original boss; 1 of 4 chosen randomly)
- **Monster action animation triggers**: all 26 actions across 6 monsters now use official anim keys (AttackSingle/Heavy/Multi/Debuff/Block/Buff/Heal/Dead/Respawn)
- **Asset manifest**: added batch 8 (18 cards) requirements, portrait background spec, missing list (22 cards)

## v1.1 (2026-08-26)

- 所有怪物动作填充动画键名 / All monster actions got animation trigger keys
- 无极汰那改为随机加入荣耀幕 Boss 池（保留原 Boss）/ Eternatus joins Glory boss pool randomly (original bosses kept)
- 马匹守护：分段转移伤害（白马→溢出黑马→玩家），防递归卡死 / Steed Guard: damage split across steeds, recursion-safe
- 喂养可复活死马 / Feeding revives dead steeds
- 无极汰那 BGM（阶段1/阶段2 无限循环）/ Eternatus BGM (phase 1/2, infinite loop)
- 调试卡（无法获得，999 伤害×1000 段）/ Debug card (unobtainable, 999 dmg × 1000 hits)
