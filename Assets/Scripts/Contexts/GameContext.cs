using System.Collections.Generic;
using UnityEngine;

public class GameContext
{
    public int AdventureNumber { get; private set; }

    public GameContext(int adventureNumber)
    {
        AdventureNumber = adventureNumber;
    }
}
