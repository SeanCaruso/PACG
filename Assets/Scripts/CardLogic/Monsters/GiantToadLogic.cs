using UnityEngine;

[EncounterLogicFor("GiantToad")]
public class GiantToadLogic : IEncounterLogic
{
    public void Execute(EncounterContext context, EncounterPhase phase)
    {
        if (phase == EncounterPhase.Resolve && !context.CheckResult.WasSuccess)
        {
            Debug.Log("Suffer the scourges Entangled and Poisoned.");
        }
    }
}
