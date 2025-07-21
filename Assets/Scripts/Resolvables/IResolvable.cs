using System.Collections.Generic;

public interface IResolvable
{
    public List<PlayCardAction> GetValidActions(ActionContext context);
}
