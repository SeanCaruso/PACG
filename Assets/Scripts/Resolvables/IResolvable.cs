using System.Collections.Generic;

public interface IResolvable
{
    public List<PlayCardAction> GetValidActions();
    public bool IsResolved(Stack<PlayCardAction> actions);
}
