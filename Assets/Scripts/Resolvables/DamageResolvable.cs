using System.Collections.Generic;
using UnityEngine;

public class DamageResolvable : IResolvable
{
    public PlayerCharacter PlayerCharacter { get; private set; }
    public int Amount { get; private set; }

    public DamageResolvable(PlayerCharacter playerCharacter, int amount)
    {
        PlayerCharacter = playerCharacter;
        Amount = amount;
    }

    public List<IStagedAction> GetValidActions()
    {
        List<IStagedAction> actions = new();

        foreach (var cardData in PlayerCharacter.hand)
        {
            actions.AddRange(GetValidActionsForCard(cardData));
        }

        return actions;
    }

    public List<IStagedAction> GetValidActionsForCard(CardData cardData)
    {
        // Grab any card-specific options for handling damage.
        var cardLogic = Game.GetPlayableLogic(cardData);
        List<IStagedAction> actions = cardLogic?.GetAvailableActions() ?? new();

        // Add default damage discard action.
        actions.Add(new DamageAction(cardData, PF.ActionType.Discard, 1));

        return actions;
    }

    public bool IsResolved(Stack<IStagedAction> actions)
    {
        // If the player's hand size is less than or equal to the damage amount, this can always be resolved by discarding the entire hand.
        if (PlayerCharacter.hand.Count <= Amount) return true;

        int totalResolved = 0;
        foreach (var action in  actions)
        {
            if (action is DamageAction damageAction)
                totalResolved += damageAction.Amount;
        }

        return totalResolved >= Amount;
    }
}
