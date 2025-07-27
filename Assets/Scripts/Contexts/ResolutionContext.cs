using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionContext
{
    private IResolvable currentResolvable;
    private bool isResolved = false;

    public ResolutionContext(IResolvable resolvable)
    {
        currentResolvable = resolvable;
    }

    public IEnumerator WaitForResolution()
    {
        isResolved = false;
        yield return new WaitUntil(() => isResolved);
    }

    public bool IsResolved(Stack<PlayCardAction> actions)
    {
        return currentResolvable.IsResolved(actions);
    }

    public bool Resolve() => isResolved = true;
}
