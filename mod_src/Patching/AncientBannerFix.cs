using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;

namespace CalyrexMod.Patching;

// 规避 StS2ZhFont（中文字体 mod）的 NAncientNameBanner patch 崩溃：
// 跳过 banner 动画（AnimateVfx），RefreshAfterAnimation 不再触发，事件 UI 正常显示
[HarmonyPatch]
public static class AncientBannerFix
{
    [HarmonyPatch(typeof(NAncientNameBanner), "AnimateVfx")]
    [HarmonyPrefix]
    private static bool AnimateVfxPrefix()
    {
        return false;
    }
}
