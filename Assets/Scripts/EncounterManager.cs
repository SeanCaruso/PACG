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
    private LogicRegistry logicRegistry;
    private IInputController inputController;
    private ResolutionManager resolutionManager;

    private void Awake()
    {
        logicRegistry = FindFirstObjectByType<LogicRegistry>();
        inputController = FindFirstObjectByType<DebugInputController>();
        resolutionManager = new(logicRegistry);
    }

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

        var actionContext = new ActionContext(context.ActivePlayer, CheckCategory.Combat, resolutionManager, logicRegistry);
        foreach (EncounterPhase phase in encounterFlow)
        {
            IEncounterLogic logic = logicRegistry.GetEncounterLogic(context.EncounteredCardData.cardID);
            var resolvables = logic?.Execute(context.ActivePlayer, phase) ?? new();

            // Resolve resolvables.
            if (resolvables.Count > 0 && resolvables[0] is CombatResolvable)
            {
                yield return resolutionManager.HandleCombatResolvable(resolvables[0] as CombatResolvable, actionContext, inputController);

                inputController.SelectedAction?.Commit();
            }
        }

        yield break;
    }

    public void ResolveDamage(List<PlayerCharacter> targets, int amount, string type)
    {

    }
}
