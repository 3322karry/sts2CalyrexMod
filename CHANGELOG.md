# Changelog / 更新日志

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
