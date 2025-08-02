using PACG.Services.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionContext
{
    public IResolvable CurrentResolvable { get; }
    private bool isResolved = false;

    public List<IStagedAction> ValidActions => CurrentResolvable?.GetValidActions() ?? new();
    public List<IStagedAction> ValidActionsForCard(CardInstance card) => CurrentResolvable?.GetValidActionsForCard(card) ?? new();

    public ResolutionContext(IResolvable resolvable)
    {
        CurrentResolvable = resolvable;

        GameEvents.ActionsCommitted += Resolve;
    }

    public IEnumerator WaitForResolution()
    {
        isResolved = false;
        yield return new WaitUntil(() => isResolved);
    }

    public bool IsResolved(List<IStagedAction> actions)
    {
        return CurrentResolvable.IsResolved(actions);
    }

    public void Resolve(List<IStagedAction> _)
    {
        isResolved = true;
        GameEvents.ActionsCommitted -= Resolve;
    }
}
