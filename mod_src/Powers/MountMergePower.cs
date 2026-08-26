using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CalyrexMod.Powers;

// 骑马合体时挂在马身上的临时标记：标记该死亡是"合体"而非战斗死亡
public sealed class MountMergePower : PowerModel
{
    public override PowerType Type => PowerType.None;

    public override PowerStackType StackType => PowerStackType.Single;
}
