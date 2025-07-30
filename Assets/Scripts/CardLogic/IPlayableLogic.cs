using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PlayableLogicForAttribute : Attribute
{
    public string CardID { get; set; }
    public PlayableLogicForAttribute(string cardID) { this.CardID = cardID; }
}

public interface IPlayableLogic : ICardLogic
{
    public bool CanPlay => GetAvailableActions().Count > 0;
    public List<IStagedAction> GetAvailableActions()
    {
        // If the card has any prohibited traits, (e.g. 2-Handed vs. Offhand), just return.
        foreach (((var character, _), var prohibitedTraits) in Game.EncounterContext?.ProhibitedTraits ?? new())
        {
            if (character == CardData.Owner && CardData.traits.Intersect(prohibitedTraits).Any())
                return new();
        }

        return GetAvailableCardActions();
    }
    public List<IStagedAction> GetAvailableCardActions();

    public void OnStage(IStagedAction action);

    public void OnUndo(IStagedAction action);

    public void Execute(IStagedAction action);
}
