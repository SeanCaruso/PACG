using System.Collections.Generic;

public interface IResolvable
{
    public List<IStagedAction> GetValidActions();
    public List<IStagedAction> GetValidActionsForCard(CardData card);
    public bool IsResolved(Stack<IStagedAction> actions);
}
