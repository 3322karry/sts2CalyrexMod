using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using CalyrexMod.Monsters;
using CalyrexMod.Powers;

namespace CalyrexMod.Relics;

public sealed class BlackWhiteCarrot : RelicModel
{
    private int _cardsPlayedThisTurn;
    private System.Collections.Generic.HashSet<int> _handCardIdsLastTurn = new();

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool AddsPet => true;

    public override bool SpawnsPets => true;

    // 本回合已打出的牌数（守住判定用）
    public bool HasPlayedCardThisTurn => _cardsPlayedThisTurn > 0;

    // 某张牌（按 combatId 哈希）是否上回合在手牌中
    public bool WasInHandLastTurn(CardModel card)
    {
        return _handCardIdsLastTurn.Contains(System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(card));
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner)
        {
            _cardsPlayedThisTurn++;
        }
        await Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(MegaCrit.Sts2.Core.Combat.CombatSide side, IReadOnlyList<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants, MegaCrit.Sts2.Core.Combat.ICombatState combatState)
    {
        if (side == MegaCrit.Sts2.Core.Combat.CombatSide.Player)
        {
            _cardsPlayedThisTurn = 0;
        }
        await Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Combat.CombatSide side, IEnumerable<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants)
    {
        if (side != MegaCrit.Sts2.Core.Combat.CombatSide.Player || base.Owner?.PlayerCombatState == null)
        {
            return;
        }
        // 记录本回合结束时的所有手牌（含保留）
        _handCardIdsLastTurn = new System.Collections.Generic.HashSet<int>();
        foreach (var card in MegaCrit.Sts2.Core.Entities.Cards.PileType.Hand.GetPile(base.Owner).Cards)
        {
            _handCardIdsLastTurn.Add(System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(card));
        }
        await Task.CompletedTask;
    }

    public override async Task BeforeCombatStart()
    {
        if (base.Owner == null || base.Owner.PlayerCombatState == null)
        {
            Log.Info("[CalyrexMod] BlackWhiteCarrot: owner/combat state missing, aborting");
            return;
        }

        try
        {
            Creature glastrier = await PlayerCmd.AddPet<Glastrier>(base.Owner);
            Creature spectrier = await PlayerCmd.AddPet<Spectrier>(base.Owner);

            // 开局喂养 11：两匹马各 +11 最大生命（当前生命同步 +11）
            await CreatureCmd.GainMaxHp(glastrier, 11m);
            await CreatureCmd.GainMaxHp(spectrier, 11m);
            Log.Info($"[CalyrexMod] BlackWhiteCarrot: fed +11 -> Glastrier hp={glastrier.CurrentHp}/{glastrier.MaxHp}, Spectrier hp={spectrier.CurrentHp}/{spectrier.MaxHp}");

            // 马匹守护（玩家侧）：蕾冠王受到的攻击伤害由马承受
            await PowerCmd.Apply<SteedGuardPassive>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 1m, base.Owner.Creature, null, silent: true);

            await PowerCmd.Apply<HeavyLance>(new ThrowingPlayerChoiceContext(), glastrier, 1m, base.Owner.Creature, null, silent: true);
            await PowerCmd.Apply<QuickSight>(new ThrowingPlayerChoiceContext(), spectrier, 1m, base.Owner.Creature, null, silent: true);
            Log.Info("[CalyrexMod] BlackWhiteCarrot: DieForYou + marks applied");
        }
        catch (Exception ex)
        {
            Log.Error($"[CalyrexMod] BlackWhiteCarrot: exception: {ex}");
        }
    }
}
