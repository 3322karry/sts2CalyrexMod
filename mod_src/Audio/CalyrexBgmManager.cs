using System;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace CalyrexMod.Audio;

// 无极汰那 Boss 战 BGM：阶段1 / 阶段2 无限循环（AudioStreamWav 内嵌字节，无需 .import）
public static class CalyrexBgmManager
{
    private static AudioStreamPlayer? _player;
    private static string? _current;

    private static Node? GetRoot()
    {
        try
        {
            var mainLoop = Engine.GetMainLoop();
            if (mainLoop is SceneTree st)
            {
                return st.Root;
            }
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] BgmManager root: {ex.Message}");
        }
        return null;
    }

    public static void PlayPhase1()
    {
        Play("res://CalyrexMod/audio/eternatus_phase1.bin");
    }

    public static void PlayPhase2()
    {
        Play("res://CalyrexMod/audio/eternatus_phase2.bin");
    }

    public static void Stop()
    {
        try
        {
            if (_player != null && GodotObject.IsInstanceValid(_player))
            {
                _player.Stop();
                _player.QueueFree();
            }
            _player = null;
            _current = null;
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] BgmManager stop: {ex.Message}");
        }
    }

    private static void Play(string binPath)
    {
        try
        {
            if (_current == binPath && _player != null && GodotObject.IsInstanceValid(_player) && _player.Playing)
            {
                return;
            }
            if (_player != null && GodotObject.IsInstanceValid(_player))
            {
                _player.Stop();
                _player.QueueFree();
            }
            _player = null;
            _current = null;

            var stream = LoadWav(binPath);
            if (stream == null)
            {
                Log.Info($"[CalyrexMod] BgmManager: failed to load {binPath}");
                return;
            }
            var root = GetRoot();
            if (root == null)
            {
                return;
            }
            _player = new AudioStreamPlayer
            {
                Stream = stream,
                VolumeDb = -8f,
                Autoplay = true,
                Name = "CalyrexBgmPlayer"
            };
            root.AddChild(_player);
            _current = binPath;
            Log.Info($"[CalyrexMod] BgmManager: playing {binPath}");
        }
        catch (Exception ex)
        {
            Log.Info($"[CalyrexMod] BgmManager play: {ex.Message}");
        }
    }

    // 读取裸 PCM 16-bit 构建 AudioStreamWav（循环）
    private static AudioStreamWav? LoadWav(string binPath)
    {
        using var file = Godot.FileAccess.Open(binPath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            Log.Info($"[CalyrexMod] BgmManager: cannot open {binPath}");
            return null;
        }
        long len = (long)file.GetLength();
        var data = file.GetBuffer((int)len);
        var wav = new AudioStreamWav
        {
            Format = AudioStreamWav.FormatEnum.Format16Bits,
            MixRate = 22050,
            Stereo = false,
            LoopMode = AudioStreamWav.LoopModeEnum.Forward,
            LoopBegin = 0,
            LoopEnd = (int)(len / 2),  // 样本数（16bit mono）
            Data = data
        };
        return wav;
    }
}
