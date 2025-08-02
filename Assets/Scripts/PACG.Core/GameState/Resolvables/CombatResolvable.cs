
using PACG.Core.Characters;
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

        foreach (var card in Character.Hand)
        {
            allOptions.AddRange(GetValidActionsForCard(card));
        }
        return allOptions;
    }

    public List<IStagedAction> GetValidActionsForCard(CardInstance card)
    {
        var cardLogic = ServiceLocator.Get<LogicRegistry>().GetPlayableLogic(card);
        return cardLogic?.GetAvailableActions() ?? new();
    }

    public bool IsResolved(List<IStagedAction> actions)
    {
        foreach (var action in actions)
        {
            if (action is PlayCardAction playAction && playAction.IsCombat)
                return true;
        }
        return false;
    }
}
