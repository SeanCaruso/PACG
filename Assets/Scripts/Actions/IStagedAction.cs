using UnityEngine;

public interface IStagedAction
{
    public CardData CardData { get; }
    public PF.ActionType ActionType { get; }
    public bool IsFreely { get; }

    public void OnStage();
    public void OnUndo();
    public void Commit();
}
