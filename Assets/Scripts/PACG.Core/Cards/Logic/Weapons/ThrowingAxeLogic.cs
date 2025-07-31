using System;
using System.Collections.Generic;
using System.Linq;

[PlayableLogicFor("ThrowingAxe")]
public class ThrowingAxeLogic : CardLogicBase, IPlayableLogic
{
    private PlayCardAction _revealAction;
    private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

    private PlayCardAction _discardAction;
    private PlayCardAction DiscardAction => _discardAction ??= new(this, Card, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

    List<IStagedAction> IPlayableLogic.GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (CanReveal) actions.Add(RevealAction);
        if (CanDiscard) actions.Add(DiscardAction);
        return actions;
    }

    readonly PF.Skill[] validSkills = { PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged };
    bool CanReveal => (
        // Reveal power can be used by the current owner while playing cards for a Strength, Dexterity, Melee, or Ranged combat check.
        Contexts.CheckContext != null
        && Contexts.CheckContext.CheckPC == Card.Owner
        && !Contexts.CheckContext.StagedCardTypes.Contains(Card.Data.cardType)
        && Contexts.CheckContext.CheckCategory == CheckCategory.Combat
        && Contexts.CheckContext.CheckPhase == CheckPhase.PlayCards
        && Contexts.CheckContext.CanPlayCardWithSkills(validSkills));

    bool CanDiscard => (
        // Discard power can be freely used on a local combat check while playing cards if the owner is proficient.
        Contexts.CheckContext != null
        && Card.Owner.IsProficient(Card.Data.cardType)
        && Contexts.CheckContext.CheckCategory == CheckCategory.Combat
        && Contexts.CheckContext.CheckPhase == CheckPhase.PlayCards
        && true); // TODO: Handle checking for local vs. distant.

    void IPlayableLogic.OnStage(IStagedAction action)
    {
        Contexts.CheckContext.RestrictValidSkills(Card, validSkills);
    }

    void IPlayableLogic.OnUndo(IStagedAction action)
    {
        Contexts.CheckContext.UndoSkillModification(Card);
    }

    void IPlayableLogic.Execute(IStagedAction action)
    {
        if (action == RevealAction)
        {
            // Reveal to use Strength, Dexterity, Melee, or Ranged + 1d8.
            var (skill, die, bonus) = Contexts.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Strength, PF.Skill.Dexterity, PF.Skill.Melee, PF.Skill.Ranged);
            Contexts.CheckContext.UsedSkill = skill;
            Contexts.CheckContext.DicePool.AddDice(1, die, bonus);
            Contexts.CheckContext.DicePool.AddDice(1, 8);
        }

        // Discard to add 1d6.
        if (action == DiscardAction)
        {
            Contexts.CheckContext.DicePool.AddDice(1, 6);
        }
    }
}
