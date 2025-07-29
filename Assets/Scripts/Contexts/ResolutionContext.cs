using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionContext
{
    public IResolvable CurrentResolvable { get; }
    private bool isResolved = false;

    public List<IStagedAction> ValidActions => CurrentResolvable?.GetValidActions() ?? new();
    public List<IStagedAction> ValidActionsForCard(CardData cardData) => CurrentResolvable?.GetValidActionsForCard(cardData) ?? new();

    public ResolutionContext(IResolvable resolvable)
    {
        CurrentResolvable = resolvable;
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

    public bool Resolve() => isResolved = true;
}
