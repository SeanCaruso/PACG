using UnityEngine;

public interface IStagedAction
{
    public CardInstance Card { get; }
    public PF.ActionType ActionType { get; }
    public bool IsFreely { get; }

    public void OnStage(CheckContext checkContext = null);
    public void OnUndo(CheckContext checkContext = null);
    public void Commit(CheckContext checkContext = null);
}
