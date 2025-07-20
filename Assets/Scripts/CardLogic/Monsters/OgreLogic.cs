using UnityEngine;

[EncounterLogicFor("Ogre")]
public class OgreLogic : IEncounterLogic
{
    public void Execute(EncounterContext context, EncounterPhase phase)
    {
        if (phase == EncounterPhase.BeforeActing && !context.CheckResult.WasSuccess)
        {
            Debug.Log("Each local character suffers 1 Combat damage.");
        }
    }
}
