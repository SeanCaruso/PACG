using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncounterPhase
{
    OnEncounter,
    Evasion,
    Villain_GuardDistant,
    BeforeActing,
    AttemptChecks,
    AfterActing,
    Resolve,
    Avenge,
    Villain_CloseLocation,
    Villain_CheckEscape,
    Villain_Defeat
}

public class EncounterManager : MonoBehaviour
{
    private bool isWaitingOnPlayer = false;

    public IEnumerator RunEncounter(EncounterContext context)
    {
        List<EncounterPhase> encounterFlow =new List<EncounterPhase>
        {
            EncounterPhase.OnEncounter,
            EncounterPhase.Evasion,
            EncounterPhase.BeforeActing,
            EncounterPhase.AttemptChecks,
            EncounterPhase.AfterActing,
            EncounterPhase.Resolve,
            EncounterPhase.Avenge,
        };

        foreach (EncounterPhase phase in encounterFlow)
        {
            IEncounterLogic logic = context.GetEncounterLogic();
            logic?.Execute(context, phase);

            while (isWaitingOnPlayer)
            {
                yield return null;
            }
        }
    }

    public void ResolveDamage(List<PlayerCharacter> targets, int amount, string type)
    {

    }
}
