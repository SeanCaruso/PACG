using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("ThrowingAxe")]
public class ThrowingAxeLogic : IPlayableLogic
{
    public CardInstance Card { get; set; }

    private PlayCardAction _revealAction;
    private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

    private PlayCardAction _discardAction;
    private PlayCardAction DiscardAction => _discardAction ??= new(this, Card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (CanReveal) actions.Add(RevealAction);
        if (CanDiscard) actions.Add(DiscardAction);
        return actions;
    }

    readonly PF.Skill[] validSkills = { PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged };
    bool CanReveal => (
        // Reveal power can be used by the current owner while playing cards for a Strength, Dexterity, Melee, or Ranged combat check.
        Game.CheckContext != null
        && Game.CheckContext.CheckPC == Card.Owner
        && !Game.CheckContext.StagedCardTypes.Contains(Card.Data.cardType)
        && Game.CheckContext.CheckCategory == CheckCategory.Combat
        && Game.CheckContext.CheckPhase == CheckPhase.PlayCards
        && Game.CheckContext.CanPlayCardWithSkills(validSkills));

    bool CanDiscard => (
        // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
        Game.CheckContext != null
        && Card.Owner.IsProficient(Card.Data.cardType)
        && Game.CheckContext.CheckCategory == CheckCategory.Combat
        && Game.CheckContext.CheckPhase == CheckPhase.PlayCards
        && true); // TODO: Handle checking for local vs. distant.

    public void OnStage(IStagedAction action)
    {
        Game.CheckContext.RestrictValidSkills(Card, validSkills);
    }

    public void OnUndo(IStagedAction action)
    {
        Game.CheckContext.UndoSkillModification(Card);
    }

    public void Execute(IStagedAction action)
    {
        if (action == RevealAction)
        {
            // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d8.
            var (skill, die, bonus) = Game.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);
        }

        // Discard to add 1d6.
        if (action == DiscardAction)
        {
            Game.CheckContext.DicePool.AddDice(1, 6);
        }
    }
}
