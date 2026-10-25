using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Logging;

namespace CalyrexMod;

/// <summary>
/// CalyrexMod 配置文件（CalyrexMod.conf，和 dll 放在同一个 mod 目录）。
/// 缺失时自动生成默认值；改动后需要重启游戏生效。
/// </summary>
public sealed class ModConfigData
{
    /// <summary>战斗结束时，是否按丰饶层数回复生命。</summary>
    [JsonPropertyName("abundance_combat_heal")]
    public bool AbundanceCombatHeal { get; set; } = true;

    /// <summary>回复比例（百分比）。

    /// 50 表示回复「丰饶层数 * 50%」的生命，向下取整。</summary>
    [JsonPropertyName("abundance_combat_heal_percent")]
    public int AbundanceCombatHealPercent { get; set; } = 50;
}

public static class ModConfig
{
    private static ModConfigData _data = new();

    public static ModConfigData Data => _data;

    public static string FilePath { get; private set; } = string.Empty;

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static void Load()
    {
        try
        {
            var dir = Path.GetDirectoryName(typeof(ModConfig).Assembly.Location);
            if (string.IsNullOrEmpty(dir))
            {
                dir = AppContext.BaseDirectory;
            }

            FilePath = Path.Combine(dir ?? ".", "CalyrexMod.conf");

            if (!File.Exists(FilePath))
            {
                Save();
                Log.Info($"[CalyrexMod] config created: {FilePath}");
                return;
            }

            string json = File.ReadAllText(FilePath);
            ModConfigData? parsed = JsonSerializer.Deserialize<ModConfigData>(json, ReadOptions);
            if (parsed != null)
            {
                _data = parsed;
            }

            Clamp();
            Log.Info($"[CalyrexMod] config loaded: abundance_combat_heal={_data.AbundanceCombatHeal}, percent={_data.AbundanceCombatHealPercent}");
        }
        catch (Exception ex)
        {
            Log.Error($"[CalyrexMod] config load failed, using defaults: {ex.Message}");
        }
    }

    public static void Save()
    {
        try
        {
            string dir = Path.GetDirectoryName(FilePath) ?? ".";
            Directory.CreateDirectory(dir);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(_data, WriteOptions));
        }
        catch (Exception ex)
        {
            Log.Error($"[CalyrexMod] config save failed: {ex.Message}");
        }
    }

    private static void Clamp()
    {
        if (_data.AbundanceCombatHealPercent < 0)
        {
            _data.AbundanceCombatHealPercent = 0;
        }
        if (_data.AbundanceCombatHealPercent > 100)
        {
            _data.AbundanceCombatHealPercent = 100;
        }
    }
}
