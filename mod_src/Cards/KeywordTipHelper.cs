using System.Collections.Generic;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public static class KeywordTipHelper
{
    public static IHoverTip AbundanceTip => HoverTipFactory.FromPower<Abundance>();
    public static IHoverTip FrozenTip => HoverTipFactory.FromPower<FrozenPower>();
    public static IHoverTip IceWallTip => HoverTipFactory.FromPower<IceWallPower>();
    public static IHoverTip EternalWhinnyTip => HoverTipFactory.FromPower<EternalWhinny>();
    public static IHoverTip QuickSightTip => HoverTipFactory.FromPower<QuickSight>();
    public static IHoverTip HeavyLanceTip => HoverTipFactory.FromPower<HeavyLance>();
    public static IHoverTip SteedGuardTip => HoverTipFactory.FromPower<SteedGuard>();
    public static IHoverTip CannotMountTip => HoverTipFactory.FromPower<CannotMountPower>();

    public static IHoverTip MountTip => new HoverTip(new LocString("cards", "KEYWORD_MOUNT.title"), new LocString("cards", "KEYWORD_MOUNT.description"));
    public static IHoverTip FeedTip => new HoverTip(new LocString("cards", "KEYWORD_FEED.title"), new LocString("cards", "KEYWORD_FEED.description"));
    public static IHoverTip MountedGlastrierTip => HoverTipFactory.FromPower<MountedGlastrier>();
    public static IHoverTip MountedSpectrierTip => HoverTipFactory.FromPower<MountedSpectrier>();

    public static IEnumerable<IHoverTip> AbundanceTips => new[] { AbundanceTip };
    public static IEnumerable<IHoverTip> FrozenTips => new[] { FrozenTip };
    public static IEnumerable<IHoverTip> MountTips => new[] { MountTip, FeedTip };
    public static IEnumerable<IHoverTip> FeedTips => new[] { FeedTip, AbundanceTip };
}
