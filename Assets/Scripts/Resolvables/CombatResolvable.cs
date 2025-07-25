
using System.Collections.Generic;
using UnityEngine;

public class CombatResolvable : IResolvable
{
    public PlayerCharacter Character { get; set; }
    public int Difficulty { get; set; }
    public CombatResolvable(PlayerCharacter character, int difficulty)
    {
        this.Character = character;
        Difficulty = difficulty;
    }

    public List<PlayCardAction> GetValidActions(ActionContext context)
    {
        var allOptions = new List<PlayCardAction>();

        foreach (var cardData in Character.hand)
        {
            var cardLogic = context.LogicRegistry.GetPlayableLogic(cardData);
            if (cardLogic != null)
            {
                var availableActions = cardLogic.GetAvailableActions(context);
                allOptions.AddRange(availableActions);
            }
        }
        return allOptions;
    }
}
