using UnityEngine;

public class TurnContext
{
    public GameContext GameContext { get; private set; }
    public CardData HourBlessing { get; private set; }
    public PlayerCharacter CurrentPC { get; private set; }

    public TurnContext(GameContext gameContext, CardData hourBlessing, PlayerCharacter currentPC)
    {
        GameContext = gameContext;
        HourBlessing = hourBlessing;
        CurrentPC = currentPC;
    }
}
