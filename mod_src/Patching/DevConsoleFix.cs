using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class DevConsoleFix
{
    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.Debug.NDevConsole), "Create")]
    [HarmonyPrefix]
    private static bool CreatePrefix(ref CanvasLayer? __result)
    {
        try
        {
            // 直接用 ResourceLoader 加载控制台场景（绕过 PreloadManager 缓存），_instance 由 _EnterTree 自动设置
            var scene = ResourceLoader.Load<PackedScene>("res://scenes/debug/dev_console.tscn", null, ResourceLoader.CacheMode.Reuse);
            if (scene == null)
            {
                Log.Info("[CalyrexMod] DevConsoleFix: dev_console.tscn failed to load");
                return true;
            }
            __result = scene.Instantiate<CanvasLayer>(PackedScene.GenEditState.Disabled);
            Log.Info("[CalyrexMod] DevConsoleFix: dev console created");
            return false;
        }
        catch (System.Exception ex)
        {
            Log.Info($"[CalyrexMod] DevConsoleFix: {ex}");
        }
        return true;
    }
}
