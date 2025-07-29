
public class DefaultDamageAction : IStagedAction
{
    public CardData CardData { get; protected set; }
    public PF.ActionType ActionType => PF.ActionType.Discard;
    public bool IsFreely => true;
    public int Amount { get; protected set; }

    public DefaultDamageAction(CardData cardData)
    {
        CardData = cardData;
    }

    public void OnStage()
    {
    }

    public void OnUndo()
    {
    }

    public void Commit()
    {
        Game.TurnContext.CurrentPC.MoveCard(CardData, ActionType);
    }
}
