using UnityEngine;

public class PlayCardAction : IStagedAction
{
    private IPlayableLogic playable;
    private PF.ActionType actionType;
    private int? powerIndex = null;
    private string label = null;

    public PlayCardAction(IPlayableLogic playable, PF.ActionType actionType, string label = null, int? powerIndex = null)
    {
        this.playable = playable;
        this.actionType = actionType;
        this.label = label;
        this.powerIndex = powerIndex;
    }

    public string GetLabel()
    {
        return label is null ? nameof(actionType) : label;
    }

    public void Commit()
    {
        playable.Execute();
    }
}
