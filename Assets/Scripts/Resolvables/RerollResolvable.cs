using System.Collections.Generic;
using UnityEngine;

public class RerollResolvable : IResolvable
{
    public PlayerCharacter PlayerCharacter { get; private set; }

    public RerollResolvable(PlayerCharacter playerCharacter)
    {
        PlayerCharacter = playerCharacter;

        // Default option is to not reroll.
        Game.CheckContext.ContextData["doReroll"] = false;
    }

    public List<IStagedAction> GetValidActions()
    {
        List<IStagedAction> actions = new();

        foreach (var cardData in PlayerCharacter.hand)
        {
            actions.AddRange(GetValidActionsForCard(cardData));
        }
        foreach (var cardData in PlayerCharacter.displayedCards)
        {
            actions.AddRange(GetValidActionsForCard(cardData));
        }

        return actions;
    }

    public List<IStagedAction> GetValidActionsForCard(CardData cardData)
    {
        var cardLogic = Game.GetPlayableLogic(cardData);
        List<IStagedAction> actions = cardLogic?.GetAvailableActions() ?? new();

        return actions;
    }

    public bool IsResolved(List<IStagedAction> actions) => true; // We can always resolve by skipping.
}
