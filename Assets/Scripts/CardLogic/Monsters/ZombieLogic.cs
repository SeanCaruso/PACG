using System.Collections.Generic;
using UnityEngine;

[EncounterLogicFor("Zombie")]
public class ZombieLogic : IEncounterLogic
{
    public List<IResolvable> Execute(EncounterContext context, EncounterPhase phase)
    {
        if (phase == EncounterPhase.AttemptChecks)
        {
            int totalDC = context.EncounteredCardData.checkRequirement.checkSteps[0].TotalDC(context.GameContext);
            return new List<IResolvable>{ new CombatResolvable(context.ActivePlayer, totalDC) };
        }

        return new();
    }
}
