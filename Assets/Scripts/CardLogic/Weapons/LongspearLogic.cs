using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longspear")]
public class LongspearLogic : IPlayableLogic
{
    public CardInstance Card { get; set; }

    private PlayCardAction _revealAction;
    private PlayCardAction RevealAction => _revealAction ??= new(this, Card, PF.ActionType.Reveal, ("IsCombat", true));

    private PlayCardAction _rerollAction;
    private PlayCardAction RerollAction => _rerollAction ??= new(this, Card, PF.ActionType.Discard, ("IsFreely", true));

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (IsCardPlayable)
        {
            if (Game.CheckContext.CheckPhase == CheckPhase.PlayCards
                && !Game.CheckContext.StagedCardTypes.Contains(Card.Data.cardType))
            {
                actions.Add(RevealAction);
            }

            // We can discard to reroll if we're in the roll dice phase and this card is one of the reroll options.
            if (Game.CheckContext.CheckPhase == CheckPhase.RollDice
                && ((List<CardInstance>)Game.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Contains(Card))
            {
                actions.Add(RerollAction);
            }
        }
        return actions;
    }

    bool IsCardPlayable => (
        // All powers on this card are specific to its owner during a Strength or Melee combat check.
        Game.CheckContext != null
        && Game.CheckContext.CheckPC == Card.Owner
        && Game.CheckContext.CheckCategory == CheckCategory.Combat
        && Game.CheckContext.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee));

    public void OnStage(IStagedAction action)
    {
        Game.EncounterContext.AddProhibitedTraits(Card.Owner, Card, "Offhand");
        Game.CheckContext.RestrictValidSkills(Card, PF.Skill.Strength, PF.Skill.Melee);
    }

    public void OnUndo(IStagedAction action)
    {
        Game.EncounterContext.UndoProhibitedTraits(Card.Owner, Card);
        Game.CheckContext.UndoSkillModification(Card);
    }

    public void Execute(IStagedAction action)
    {
        if (!Game.CheckContext.ContextData.ContainsKey("rerollCards"))
            Game.CheckContext.ContextData["rerollCards"] = new List<CardData>();
        List<CardInstance> rerollSources = (List<CardInstance>)Game.CheckContext.ContextData["rerollCards"];

        // Reveal to use Strength or Melee + 1d8.
        if (action == RevealAction)
        {
            (PF.Skill skill, int die, int bonus) = Card.Owner.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);

            rerollSources.Add(Card);
        }

        // Discard to reroll.
        if (action == RerollAction)
        {
            rerollSources.Remove(Card);
            Game.CheckContext.ContextData["doReroll"] = true;
        }
    }
}
