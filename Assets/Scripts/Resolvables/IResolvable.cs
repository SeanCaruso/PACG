using System.Collections.Generic;

public interface IResolvable
{
    public List<IStagedAction> GetValidActions();
    public List<IStagedAction> GetValidActionsForCard(CardInstance card);
    public bool IsResolved(List<IStagedAction> actions);
}
