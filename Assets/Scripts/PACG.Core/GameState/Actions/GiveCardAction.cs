using System.Collections.Generic;
using UnityEngine;

// TODO: Implement giving cards when we have multiple PCs.
public class GiveCardAction : IStagedAction
{
    public CardInstance Card { get; }
    public PF.ActionType ActionType => PF.ActionType.Discard;
    public bool IsFreely => false;

    private readonly PlayerCharacter _targetPc;

    public GiveCardAction(CardInstance card, PlayerCharacter targetPc)
    {
        Card = card;
        _targetPc = targetPc;
    }

    public void Commit(CheckContext _ = null)
    {
        string targetName = _targetPc?.characterData.characterName ?? "nonexistent PC";
        Debug.Log($"{Card.Data.cardName} given to {targetName}.");
    }

    public void OnStage(CheckContext _ = null)
    {
        ServiceLocator.Get<ActionStagingManager>().StageAction(this);
    }

    public void OnUndo(CheckContext _ = null)
    {
        ServiceLocator.Get<ActionStagingManager>().Cancel();
    }
}
