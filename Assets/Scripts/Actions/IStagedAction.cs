using UnityEngine;

public interface IStagedAction
{
    public CardData CardData { get; }
    public PF.ActionType ActionType { get; }

    public void Commit();
}
