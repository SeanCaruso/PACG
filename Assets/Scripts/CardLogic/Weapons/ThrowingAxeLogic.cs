using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("ThrowingAxe")]
public class ThrowingAxeLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private const int RevealIndex = 0;
    private const int DiscardIndex = 1;

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (CanReveal)
        {
            actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Reveal, powerIndex: RevealIndex, isCombat: true));
        }

        if (CanDiscard)
        {
            actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Discard, powerIndex: DiscardIndex, isCombat: true, isFreely: true));
        }
        return actions;
    }

    bool CanReveal => (
        // Reveal power can be used by the current owner while playing cards for a combat check.
        Game.CheckContext.CheckPC == CardData.Owner &&
        !Game.CheckContext.StagedCardTypes.Contains(CardData.cardType) &&
        Game.CheckContext.CheckCategory == CheckCategory.Combat &&
        Game.CheckContext.CheckPhase == CheckPhase.PlayCards
        );

    bool CanDiscard => (
        // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
        CardData.Owner.IsProficient(CardData.cardType) &&
        Game.CheckContext.CheckCategory == CheckCategory.Combat &&
        Game.CheckContext.CheckPhase == CheckPhase.PlayCards &&
        true // TODO: Handle checking for local vs. distant.
        );

    public void OnStage(int? powerIndex = null)
    {
        Game.Stage(CardData, isFreely: powerIndex == DiscardIndex);
    }

    public void OnUndo(int? powerIndex = null)
    {
        Game.Undo(CardData, isFreely: powerIndex == DiscardIndex);
    }

    public void ExecuteCardLogic(int? powerIndex = null)
    {
        if (powerIndex == RevealIndex)
        {
            // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d8.
            var (skill, die, bonus) = Game.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);
        }

        // Discard to add 1d6.
        if (powerIndex == DiscardIndex)
        {
            Game.CheckContext.DicePool.AddDice(1, 6);
        }
    }
}
