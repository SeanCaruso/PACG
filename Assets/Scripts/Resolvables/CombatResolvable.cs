
using System.Collections.Generic;
using UnityEngine;

public class CombatResolvable : IResolvable
{
    public PlayerCharacter Character { get; protected set; }
    public int Difficulty { get; protected set; }
    public CombatResolvable(PlayerCharacter character, int difficulty)
    {
        this.Character = character;
        Difficulty = difficulty;
    }

    public List<IStagedAction> GetValidActions()
    {
        var allOptions = new List<IStagedAction>();

        foreach (var cardData in Character.hand)
        {
            allOptions.AddRange(GetValidActionsForCard(cardData));
        }
        return allOptions;
    }

    public List<IStagedAction> GetValidActionsForCard(CardData cardData)
    {
        var cardLogic = Game.GetPlayableLogic(cardData);
        return cardLogic?.GetAvailableActions() ?? new();
    }

    public bool IsResolved(Stack<IStagedAction> actions)
    {
        foreach (var action in actions)
        {
            if (action is PlayCardAction playAction && playAction.isCombat)
                return true;
        }
        return false;
    }
}
