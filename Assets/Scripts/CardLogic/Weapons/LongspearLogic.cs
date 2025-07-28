using System;
using System.Collections.Generic;
using UnityEngine;

[PlayableLogicFor("Longspear")]
public class LongspearLogic : IPlayableLogic
{
    public CardData CardData { get; set; }

    private const int RevealIndex = 0;
    private const int DiscardIndex = 1;

    public List<IStagedAction> GetAvailableCardActions()
    {
        List<IStagedAction> actions = new();
        if (IsCardPlayable)
        {
            if (Game.CheckContext.CheckPhase == CheckPhase.PlayCards
                && !Game.CheckContext.StagedCardTypes.Contains(CardData.cardType))
            {
                actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Reveal, powerIndex: RevealIndex, isCombat: true));
            }

            // We can discard to reroll if we're in the roll dice phase and this card is one of the reroll options.
            if (Game.CheckContext.CheckPhase == CheckPhase.RollDice
                && ((List<CardData>)Game.CheckContext.ContextData.GetValueOrDefault("rerollCardData", new List<CardData>())).Contains(CardData))
            {
                actions.Add(new PlayCardAction(this, CardData, PF.ActionType.Discard, powerIndex: DiscardIndex));
            }
        }
        return actions;
    }

    bool IsCardPlayable => (
        // All powers on this card are specific to its owner during a combat check.
        Game.CheckContext.CheckPC == CardData.Owner &&
        Game.CheckContext.CheckCategory == CheckCategory.Combat
    );

    public void OnStage(int? powerIndex = null)
    {
        Game.EncounterContext.AddProhibitedTraits(CardData.Owner, CardData, "Offhand");
        if (powerIndex == RevealIndex) Game.Stage(CardData);
    }

    public void OnUndo(int? powerIndex = null)
    {
        Game.EncounterContext.ProhibitedTraits.Remove((CardData.Owner, CardData));
        if (powerIndex == RevealIndex) Game.Undo(CardData);
    }

    public void ExecuteCardLogic(int? powerIndex = null)
    {
        if (!Game.CheckContext.ContextData.ContainsKey("rerollCardData"))
            Game.CheckContext.ContextData["rerollCardData"] = new List<CardData>();
        List<CardData> rerollSources = (List<CardData>)Game.CheckContext.ContextData["rerollCardData"];

        // Reveal to use Strength or Melee + 1d8.
        if (powerIndex == RevealIndex)
        {
            (PF.Skill skill, int die, int bonus) = CardData.Owner.GetBestSkill(PF.Skill.Strength, PF.Skill.Melee);
            Game.CheckContext.UsedSkill = skill;
            Game.CheckContext.DicePool.AddDice(1, die, bonus);
            Game.CheckContext.DicePool.AddDice(1, 8);

            rerollSources.Add(CardData);
        }

        // Discard to reroll.
        if (powerIndex == DiscardIndex)
        {
            rerollSources.Remove(CardData);
            Game.CheckContext.ContextData["doReroll"] = true;
        }
    }
}
