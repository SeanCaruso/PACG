using System.Collections.Generic;
using UnityEngine;

public class GameContext
{
    public LogicRegistry LogicRegistry { get; }
    public List</*PlayerCharacter*/Deck> AllPlayers { get; }

    public GameContext(LogicRegistry logicRegistry, List<Deck> allPlayers)
    {
        LogicRegistry = logicRegistry;
        AllPlayers = allPlayers;
    }
}
