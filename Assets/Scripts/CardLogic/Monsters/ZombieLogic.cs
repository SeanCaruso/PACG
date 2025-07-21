using System.Collections.Generic;
using UnityEngine;

[EncounterLogicFor("Zombie")]
public class ZombieLogic : IEncounterLogic
{
    public List<IResolvable> Execute(PlayerCharacter character, EncounterPhase phase)
    {
        if (phase == EncounterPhase.AttemptChecks)
        {
            return new List<IResolvable>{ new CombatResolvable(character, 9) };
        }

        return new();
    }
}
