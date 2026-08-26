using System.Collections;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using CalyrexMod.CardPools;
using CalyrexMod.Characters;

namespace CalyrexMod.Patching;

[HarmonyPatch]
public static class CardLibraryFix
{
    private const string CalyrexPoolNodeName = "CalyrexPool";
    private static bool _buttonCreated;

    // 构造时就把蕾冠王加入字典（占位 null），确保任何后续访问不抛 KeyNotFound
    [HarmonyPatch(typeof(NCardLibrary), MethodType.Constructor)]
    [HarmonyPostfix]
    private static void CtorPostfix(NCardLibrary __instance)
    {
        try
        {
            var filtersObj = HarmonyLib.Traverse.Create(__instance).Field("_cardPoolFilters").GetValue() as IDictionary;
            if (filtersObj == null)
            {
                return;
            }
            var calyrex = ModelDb.Character<CalyrexCharacter>();
            if (!filtersObj.Contains(calyrex))
            {
                filtersObj[calyrex] = null;
                Log.Info("[CalyrexMod] CardLibraryFix: ctor added Calyrex placeholder");
            }
        }
        catch (System.Exception ex)
        {
            Log.Info($"[CalyrexMod] CardLibraryFix ctor: {ex}");
        }
    }

    // 打开图鉴时：创建独立按钮并替换占位值为按钮
    [HarmonyPatch(typeof(NCardLibrary), "OnSubmenuOpened")]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void OnSubmenuOpenedPostfix(NCardLibrary __instance)
    {
        try
        {
            EnsureCalyrexFilterButton(__instance);

            var calyrex = ModelDb.Character<CalyrexCharacter>();
            var calyrexButton = __instance.GetNodeOrNull<NCardPoolFilter>(CalyrexPoolNodeName);
            var filtersObj = HarmonyLib.Traverse.Create(__instance).Field("_cardPoolFilters").GetValue() as IDictionary;
            if (filtersObj != null && calyrexButton != null)
            {
                filtersObj[calyrex] = calyrexButton;
            }
        }
        catch (System.Exception ex)
        {
            Log.Info($"[CalyrexMod] CardLibraryFix: {ex}");
        }
    }

    private static void EnsureCalyrexFilterButton(NCardLibrary __instance)
    {
        if (_buttonCreated || __instance.GetNodeOrNull<NCardPoolFilter>(CalyrexPoolNodeName) != null)
        {
            _buttonCreated = true;
            return;
        }
        var colorless = __instance.GetNodeOrNull<NCardPoolFilter>("%ColorlessPool");
        if (colorless == null)
        {
            return;
        }
        var container = colorless.GetParent();
        if (container == null)
        {
            return;
        }

        var scene = ResourceLoader.Load<PackedScene>("res://scenes/screens/card_library/library_pool_toggle.tscn", null, ResourceLoader.CacheMode.Reuse);
        if (scene == null)
        {
            return;
        }
        var button = scene.Instantiate<NCardPoolFilter>(PackedScene.GenEditState.Disabled);
        button.Name = CalyrexPoolNodeName;
        container.AddChildSafely(button);
        container.MoveChildSafely(button, colorless.GetIndex() + 1);

        var image = button.GetNodeOrNull<TextureRect>("Image");
        if (image != null)
        {
            // 筛选图标 = 费用图标（皇冠）
            var tex = ResourceLoader.Load<Texture2D>("res://CalyrexMod/icons/energy_calyrex.tres", null, ResourceLoader.CacheMode.Reuse);
            if (tex != null)
            {
                image.Texture = tex;
            }
        }

        var method = HarmonyLib.AccessTools.Method(typeof(NCardLibrary), "UpdateCardPoolFilter");
        if (method != null)
        {
            var action = (System.Action<NCardPoolFilter>)method.CreateDelegate(typeof(System.Action<NCardPoolFilter>), __instance);
            button.Connect(NCardPoolFilter.SignalName.Toggled, Callable.From(action));
        }

        var poolFilters = HarmonyLib.Traverse.Create(__instance).Field("_poolFilters").GetValue() as IDictionary;
        if (poolFilters != null)
        {
            poolFilters[button] = new System.Func<MegaCrit.Sts2.Core.Models.CardModel, bool>(c => c.Pool is CalyrexCardPool);
        }

        _buttonCreated = true;
        Log.Info("[CalyrexMod] CardLibraryFix: created Calyrex filter button");
    }
}
