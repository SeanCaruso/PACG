using UnityEngine;

public class TurnContext
{
    public CardData HourBlessing { get; private set; }
    public PlayerCharacter CurrentPC { get; private set; }

    public TurnContext(CardData hourBlessing, PlayerCharacter currentPC)
    {
        HourBlessing = hourBlessing;
        CurrentPC = currentPC;
    }
}
