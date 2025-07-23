using UnityEngine;

public class PlayCardAction : IStagedAction
{
    private BasePlayableLogic playable;
    private CardData cardData;
    private PF.ActionType actionType;
    private int? powerIndex = null;
    private string label = null;

    public PlayCardAction(BasePlayableLogic playable, CardData cardData, PF.ActionType actionType, string label = null, int? powerIndex = null)
    {
        this.playable = playable;
        this.cardData = cardData;
        this.actionType = actionType;
        this.label = label;
        this.powerIndex = powerIndex;
    }

    public string GetLabel()
    {
        return $"{(label is null ? actionType.ToString() : label)} {cardData.cardName}";
    }

    public void Commit(ActionContext context)
    {
        context.Traits.AddRange(cardData.traits);
        playable.Execute(context, powerIndex);

        context.TurnContext.CurrentPC.MoveCard(cardData, actionType);
    }
}
