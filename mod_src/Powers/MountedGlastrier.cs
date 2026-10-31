using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

public sealed class MountedGlastrier : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 合体时马身上的「重装之矛」层数：解除合体后还给新生成的雪暴马（不动了玩家身上的显示）
    public int MarkLayers;
}
