using UnityEngine;

public class TurnContext
{
    public CardInstance HourBlessing { get; private set; }
    public PlayerCharacter CurrentPC { get; private set; }

    public TurnContext(CardInstance hourBlessing, PlayerCharacter currentPC)
    {
        HourBlessing = hourBlessing;
        CurrentPC = currentPC;
    }
}
