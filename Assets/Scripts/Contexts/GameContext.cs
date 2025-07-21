using System.Collections.Generic;
using UnityEngine;

public class GameContext
{
    public IInputController InputController { get; }
    public LogicRegistry LogicRegistry { get; }
    public List<PlayerCharacter> AllPlayers { get; }

    public GameContext(IInputController inputController, LogicRegistry logicRegistry, List<PlayerCharacter> allPlayers)
    {
        InputController = inputController;
        LogicRegistry = logicRegistry;
        AllPlayers = allPlayers;
    }
}
