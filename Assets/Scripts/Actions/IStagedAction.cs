using UnityEngine;

public interface IStagedAction
{
    public void Commit(ActionContext context);
}
