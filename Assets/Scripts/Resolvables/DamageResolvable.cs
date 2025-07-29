using System.Collections.Generic;
using UnityEngine;

public class DamageResolvable : IResolvable
{
    public PlayerCharacter PlayerCharacter { get; private set; }
    public string DamageType { get; set; }
    public int Amount { get; private set; }

    public DamageResolvable(PlayerCharacter playerCharacter, int amount, string damageType = "Combat")
    {
        PlayerCharacter = playerCharacter;
        Amount = amount;
        DamageType = damageType;
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

        // Add default damage discard action if the card was in the player's hand.
        if (PlayerCharacter.hand.Contains(cardData))
            actions.Add(new DefaultDamageAction(cardData));

        return actions;
    }

    public bool IsResolved(List<IStagedAction> actions)
    {
        // If the player's hand size is less than or equal to the damage amount, this can always be resolved by discarding the entire hand.
        //if (PlayerCharacter.hand.Count <= Amount) return true;

        // This was presenting issues, so require manually discarding everything for now.
        if (PlayerCharacter.hand.Count == 0) return true;

        int totalResolved = 0;
        foreach (var action in  actions)
        {
            if (action is DefaultDamageAction)
                totalResolved += 1;
            else if (action is PlayCardAction playAction)
                totalResolved += (int)playAction.ActionData.GetValueOrDefault("Damage", 0);
        }

        return totalResolved >= Amount;
    }
}
