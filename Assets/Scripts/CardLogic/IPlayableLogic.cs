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
        if (CardData.traits.Intersect(Game.EncounterContext.ProhibitedTraits).Any())
            return new();

        return GetAvailableCardActions();
    }
    public List<IStagedAction> GetAvailableCardActions();

    public void OnStage(int? powerIndex = null);

    public void OnUndo(int? powerIndex = null);

    public void Execute(int? powerIndex = null)
    {
        Game.CheckContext.StagedCards.Add(CardData);
        ExecuteCardLogic(powerIndex);
    }

    public void ExecuteCardLogic(int? powerIndex = null);
}
