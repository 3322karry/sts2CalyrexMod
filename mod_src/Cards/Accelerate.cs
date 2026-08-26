using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using CalyrexMod.Powers;

namespace CalyrexMod.Cards;

public sealed class Accelerate : CardModel
{
    public Accelerate()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardPoolModel Pool => ModelDb.CardPool<CalyrexMod.CardPools.CalyrexCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<AcceleratePower>();
        }
    }

    // 只有骑灵幽马（黑马）时才能使用
    protected override bool IsPlayable => base.Owner != null && base.Owner.Creature.Powers.Any((PowerModel p) => p is MountedSpectrier);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 从手牌选一张牌：重放 2 + 消耗
        var hand = PileType.Hand.GetPile(base.Owner);
        var handPrefs = new CardSelectorPrefs(new LocString("cards", "ACCELERATE.handSelectionPrompt"), 1);
        var handCard = (await CardSelectCmd.FromHand(choiceContext, base.Owner, handPrefs, (CardModel c) => c != this && c.IsRemovable && c.Type != CardType.Curse, this)).FirstOrDefault();
        if (handCard != null)
        {
            handCard.BaseReplayCount += 2;
            handCard.AddKeyword(CardKeyword.Exhaust);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Accelerate: hand card {handCard.Id.Entry} replay={handCard.BaseReplayCount}");
        }

        // 从抽牌堆选一张牌：重放 2 + 消耗
        var draw = PileType.Draw.GetPile(base.Owner);
        var drawPrefs = new CardSelectorPrefs(new LocString("cards", "ACCELERATE.drawSelectionPrompt"), 1);
        var drawCard = (await CardSelectCmd.FromCombatPile(choiceContext, draw, base.Owner, drawPrefs, (CardModel c) => c.IsRemovable && c.Type != CardType.Curse)).FirstOrDefault();
        if (drawCard != null)
        {
            drawCard.BaseReplayCount += 2;
            drawCard.AddKeyword(CardKeyword.Exhaust);
            MegaCrit.Sts2.Core.Logging.Log.Info($"[CalyrexMod] Accelerate: draw card {drawCard.Id.Entry} replay={drawCard.BaseReplayCount}");
        }

        // 每使用 2 费，获得 1 费 1 力（升级后 2 力）
        int layers = base.IsUpgraded ? 2 : 1;
        await PowerCmd.Apply<AcceleratePower>(choiceContext, base.Owner.Creature, layers, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}
