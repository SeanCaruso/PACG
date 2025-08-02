using PACG.Services.Game;
using System.Collections.Generic;
using UnityEngine;

[EncounterLogicFor("Zombie")]
public class ZombieLogic : CardLogicBase, IEncounterLogic
{
    public List<IResolvable> Execute(EncounterPhase phase)
    {
        if (phase == EncounterPhase.AttemptChecks)
        {
            return new List<IResolvable>{ new CombatResolvable(Contexts.TurnContext.CurrentPC, Contexts.CheckContext.TotalDC) };
        }

        return new();
    }
}
