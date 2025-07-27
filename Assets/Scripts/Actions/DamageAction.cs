
public class DamageAction : IStagedAction
{
    public CardData CardData { get; protected set; }
    public PF.ActionType ActionType { get; protected set; }
    public int Amount { get; protected set; }

    public DamageAction(CardData cardData, PF.ActionType actionType, int amount)
    {
        CardData = cardData;
        ActionType = actionType;
        Amount = amount;
    }

    public void Commit()
    {
        Game.TurnContext.CurrentPC.MoveCard(CardData, ActionType);
    }
}
