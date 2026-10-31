using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

public sealed class MountedSpectrier : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 合体时马身上的「迅捷之视」层数：解除合体后还给新生成的灵幽马
    public int MarkLayers;
}
