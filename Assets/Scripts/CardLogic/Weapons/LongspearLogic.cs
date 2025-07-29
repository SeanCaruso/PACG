using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longspear")]
public class LongspearLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private PlayCardAction _revealAction;
    private PlayCardAction RevealAction => _revealAction ??= new(this, CardData, PF.ActionType.Reveal, ("IsCombat", true));

    private PlayCardAction _rerollAction;
    private PlayCardAction RerollAction => _rerollAction ??= new(this, CardData, PF.ActionType.Discard, ("IsFreely", true));

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (IsCardPlayable)
        {
            if (Game.CheckContext.CheckPhase == CheckPhase.PlayCards
                && !Game.CheckContext.StagedCardTypes.Contains(CardData.cardType))
            {
                actions.Add(RevealAction);
            }

            // We can discard to reroll if we're in the roll dice phase and this card is one of the reroll options.
            if (Game.CheckContext.CheckPhase == CheckPhase.RollDice
                && ((List<CardData>)Game.CheckContext.ContextData.GetValueOrDefault("rerollCardData", new List<CardData>())).Contains(CardData))
            {
                actions.Add(RerollAction);
            }
        }
        return actions;
    }

    bool IsCardPlayable => (
        // All powers on this card are specific to its owner during a Strength or Melee combat check.
        Game.CheckContext.CheckPC == CardData.Owner &&
        Game.CheckContext.CheckCategory == CheckCategory.Combat &&
        Game.CheckContext.CanPlayCardWithSkills(PF.Skill.Strength, PF.Skill.Melee)
    );

    public void OnStage(IStagedAction action)
    {
        Game.EncounterContext.AddProhibitedTraits(CardData.Owner, CardData, "Offhand");
        Game.CheckContext.RestrictValidSkills(CardData, PF.Skill.Strength, PF.Skill.Melee);
    }

    public void OnUndo(IStagedAction action)
    {
        Game.EncounterContext.UndoProhibitedTraits(CardData.Owner, CardData);
        Game.CheckContext.UndoSkillModification(CardData);
    }

    public void Execute(IStagedAction action)
    {
        if (!Game.CheckContext.ContextData.ContainsKey("rerollCardData"))
            Game.CheckContext.ContextData["rerollCardData"] = new List<CardData>();
        List<CardData> rerollSources = (List<CardData>)Game.CheckContext.ContextData["rerollCardData"];

        // Reveal to use Strength or Melee + 1d8.
        if (action == RevealAction)
        {
            (PF.Skill skill, int die, int bonus) = CardData.Owner.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);

            rerollSources.Add(CardData);
        }

        // Discard to reroll.
        if (action == RerollAction)
        {
            rerollSources.Remove(CardData);
            Game.CheckContext.ContextData["doReroll"] = true;
        }
    }
}
