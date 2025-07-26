using System.Collections.Generic;
using UnityEngine;

[EncounterLogicFor("Zombie")]
public class ZombieLogic : IEncounterLogic
{
    public CardData CardData { get; set; }

    public List<IResolvable> Execute(EncounterPhase phase)
    {
        if (phase == EncounterPhase.AttemptChecks)
        {
            int totalDC = Game.EncounterContext.EncounteredCardData.checkRequirement.checkSteps[0].TotalDC;
            return new List<IResolvable>{ new CombatResolvable(Game.TurnContext.CurrentPC, totalDC) };
        }

        return new();
    }
}
