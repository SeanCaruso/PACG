using System.Collections.Generic;
using UnityEngine;

public class GiveCardResolvable : IResolvable
{
    private readonly PlayerCharacter _targetPc;

    public GiveCardResolvable(PlayerCharacter targetPc)
    {
        _targetPc = targetPc;
    }

    public List<IStagedAction> GetValidActions()
    {
        List<IStagedAction> actions = new();

        var turnContext = ServiceLocator.Get<ContextManager>().TurnContext;
        foreach (var card in turnContext.CurrentPC.Hand)
        {
            actions.AddRange(GetValidActionsForCard(card));
        }

        return actions;
    }

    public List<IStagedAction> GetValidActionsForCard(CardInstance card)
    {
        return new List<IStagedAction> { new GiveCardAction(card, _targetPc) };
    }

    public bool IsResolved(List<IStagedAction> actions)
    {
        // We can always resolve.
        return true;
    }
}
