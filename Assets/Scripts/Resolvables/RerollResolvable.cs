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

        foreach (var card in PlayerCharacter.Hand)
        {
            actions.AddRange(GetValidActionsForCard(card));
        }
        foreach (var cardData in PlayerCharacter.DisplayedCards)
        {
            actions.AddRange(GetValidActionsForCard(cardData));
        }

        return actions;
    }

    public List<IStagedAction> GetValidActionsForCard(CardInstance card)
    {
        var cardLogic = Game.GetPlayableLogic(card);
        List<IStagedAction> actions = cardLogic?.GetAvailableActions() ?? new();

        return actions;
    }

    public bool IsResolved(List<IStagedAction> actions) => true; // We can always resolve by skipping.
}
