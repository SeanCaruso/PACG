using System.Collections.Generic;
using UnityEngine;

public class PlayCardAction : IStagedAction
{
    // Data common to all staged actions.
    public IPlayableLogic Playable { get; private set; }
    public CardData CardData { get; private set; }
    public PF.ActionType ActionType { get; private set; }

    // Dictionary to hold any custom data.
    public Dictionary<string, object> ActionData { get; } = new();

    private readonly string label = null;

    public PlayCardAction(IPlayableLogic playable, CardData cardData, PF.ActionType actionType, params (string, object)[] actionData)
    {
        this.Playable = playable;
        this.CardData = cardData;
        this.ActionType = actionType;

        foreach ((string key, object value) in actionData)
            ActionData.Add(key, value);
    }

    // Convenience methods
    public bool IsCombat => (bool)ActionData.GetValueOrDefault("IsCombat", false);
    public bool IsFreely => (bool)ActionData.GetValueOrDefault("IsFreely", false);

    public string GetLabel()
    {
        return $"{(label is null ? ActionType.ToString() : label)} {CardData.cardName}";
    }

    public void OnStage()
    {
        Game.Stage(this);
        Playable?.OnStage(this);
    }

    public void OnUndo()
    {
        Game.Undo(this);
        Playable?.OnUndo(this);
    }

    public void Commit()
    {
        Game.CheckContext?.Traits.AddRange(CardData.traits);
        Playable.Execute(this);
        // The card data on the PlayerCharacter was moved during staging, so don't do it here.
    }
}
