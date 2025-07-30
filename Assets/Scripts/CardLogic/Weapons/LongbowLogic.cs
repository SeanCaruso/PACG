using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longbow")]
public class LongbowLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private PlayCardAction _revealAction;
    private PlayCardAction RevealAction => _revealAction ??= new(this, CardData, PF.ActionType.Reveal, ("IsCombat", true));

    private PlayCardAction _discardAction;
    private PlayCardAction DiscardAction => _discardAction ??= new(this, CardData, PF.ActionType.Discard, ("IsCombat", true), ("IsFreely", true));

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (CanReveal) actions.Add(RevealAction);
        if (CanDiscard) actions.Add(DiscardAction);
        return actions;
    }

    bool CanReveal => (
        // Reveal power can be used by the current owner while playing cards for a Dexterity or Ranged combat check.
        Game.CheckContext != null
        && Game.CheckContext.CheckPC == CardData.Owner
        && !Game.CheckContext.StagedCardTypes.Contains(CardData.cardType)
        && Game.CheckContext.CheckCategory == CheckCategory.Combat
        && Game.CheckContext.CheckPhase == CheckPhase.PlayCards
        && Game.CheckContext.CanPlayCardWithSkills(PF.Skill.Dexterity, PF.Skill.Ranged)
        );

    bool CanDiscard => (
        // Discard power can be freely used on an another character's combat check while playing cards if the owner is proficient.
        Game.CheckContext != null
        && CardData.Owner != Game.CheckContext.CheckPC
        && CardData.Owner.IsProficient(CardData.cardType)
        && Game.CheckContext.CheckCategory == CheckCategory.Combat
        && Game.CheckContext.CheckPhase == CheckPhase.PlayCards
        );

    public void OnStage(IStagedAction action)
    {
        if (action == RevealAction)
            Game.CheckContext.RestrictValidSkills(CardData, PF.Skill.Dexterity, PF.Skill.Ranged);

        Game.EncounterContext.AddProhibitedTraits(CardData.Owner, CardData, "Offhand");
    }

    public void OnUndo(IStagedAction action)
    {
        Game.CheckContext.UndoSkillModification(CardData);
        Game.EncounterContext.ProhibitedTraits.Remove((CardData.Owner, CardData));
    }

    public void Execute(IStagedAction action)
    {
        if (action == RevealAction)
        {
            // Reveal to use Dexterity or Ranged + 1d8.
            var (skill, die, bonus) = Game.TurnContext.CurrentPC.GetBestSkill(PF.Skill.Dexterity, PF.Skill.Ranged);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);
        }

        // Discard to add 1d6.
        if (action == DiscardAction)
        {
            Game.CheckContext.DicePool.AddDice(1, 8);
        }
    }
}
