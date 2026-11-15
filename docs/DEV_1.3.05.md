# 1.3.05 开发计划（进行中，未发布）

> 规则：**在用户明确要求发布之前，不跑 release.py、不推版本到工坊、不改线上条目。**
> 分支：`dev-1.3.05`（从 `beta-0.113` 拉出）。

## 目标基线

| 项 | 值 |
|---|---|
| 当前游戏 | **v0.113.1**（public-beta，commit d09d48e5） |
| 开发版本号 | `v1.3.05.113.1` |
| 引用 DLL | `libs/game-0.113.1/`（sts2/0Harmony/GodotSharp/SmartFormat） |
| 构建命令 | `dotnet build CalyrexMod.csproj -c Release -p:GameVersion=113 -p:GameLibsDir=libs\game-0.113.1` |
| 部署 | `python tools/deploy.py`（需先关游戏） |

早期基线 `libs/game-0.107.0` 与 `libs/game-0.113.0` 仍保留，切回稳定版时把 `libs` 指回对应目录并去掉 `GAME_113`。

## 已完成的启动工作（2026-11-15）

- 0.113.0 → 0.113.1 的 API 差异：**0 编译错误**，直接兼容。
- 版本号提升到 `v1.3.05.113.1`，构建 0 错误，0.113.1 客户端加载初始化无异常。
- 启动验证：`Finished mod initialization for 'Calyrex Mod'`、`v1.3.05.113.1`，无 `DuplicateModelException`。

## 待办候选（待用户确认优先级）

1. **玩法回归验证**：0.113 下 STS2_MCP 0.4.0 不可用（缺 `LobbyPlayer`），需要替代测试手段：
   - 方案 A：用户手动试玩并反馈（最快，覆盖 UI/手感）；
   - 方案 B：升级/移植 STS2_MCP（第三方，工作量大）；
   - 方案 C：临时写一个只读状态导出 patch（仅测试用）。
2. **0.107 稳定版构建恢复**：当前调用点已按 0.113 签名改写（`FromCard(..., null)`、`Damage(..., null)`），0.107 无法直接编译；如需继续维护稳定版，需要改成兼容封装或 `#if`。
3. **遗物存档状态验证**：0.113 移除了 `SavedPropertiesTypeCache`，改为内置 `ModelIdSerializationCache`；需确认 mod 遗物（FocusSash/Multiscale/…）的 `[SavedProperty]` 仍能正确序列化。
4. **内容/数值迭代**：等用户给出 1.3.05 的功能清单（新卡、平衡、事件、Boss 等）。

## 工作日志

- 2026-11-15：建立 `dev-1.3.05` 分支；升级基线到 0.113.1；版本号 v1.3.05.113.1；构建/加载验证通过。
