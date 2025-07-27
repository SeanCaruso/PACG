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
    private UIInputController inputController;

    private void Awake()
    {
        logicRegistry = FindFirstObjectByType<LogicRegistry>();
        inputController = FindFirstObjectByType<UIInputController>();
    }

    public IEnumerator RunEncounter()
    {
        List<EncounterPhase> encounterFlow = new List<EncounterPhase>
        {
            EncounterPhase.OnEncounter,
            EncounterPhase.Evasion,
            EncounterPhase.BeforeActing,
            EncounterPhase.AttemptChecks,
            EncounterPhase.AfterActing,
            EncounterPhase.Resolve,
            EncounterPhase.Avenge,
        };

        EncounterContext context = Game.EncounterContext;

        Game.NewCheck(new(CheckCategory.Combat, logicRegistry));
        Game.CheckContext.ContextData["EncounteredCard"] = context.EncounteredCardData;
        foreach (EncounterPhase phase in encounterFlow)
        {
            IEncounterLogic logic = logicRegistry.GetEncounterLogic(context.EncounteredCardData.cardID);
            var resolvables = logic?.Execute(phase) ?? new();

            // Resolve resolvables.
            if (resolvables.Count > 0 && resolvables[0] is CombatResolvable)
            {
                CombatResolvable combatResolvable = resolvables[0] as CombatResolvable;
                Game.NewResolution(new(combatResolvable));
                yield return Game.ResolutionContext.WaitForResolution();
                Game.EndResolution();

                ResolveCombatCheck(combatResolvable.Difficulty);
            }
        }
        Game.EndCheck();

        yield break;
    }

    private CheckResult ResolveCombatCheck(int dc)
    {
        CheckContext context = Game.CheckContext;

        // Add blessing dice.
        context.DicePool.AddDice(context.BlessingCount, Game.TurnContext.CurrentPC.GetSkill(context.UsedSkill).die);

        int rollResult = context.DicePool.Roll();

        CheckResult checkResult = new(rollResult, dc, Game.TurnContext.CurrentPC, context.UsedSkill, context.Traits);

        if (checkResult.WasSuccess)
        {
            Debug.Log($"Rolled {rollResult} vs. {dc} - Success!");
        }
        else
        {
            Debug.Log($"Rolled {rollResult} vs. {dc} - Take {checkResult.MarginOfSuccess * -1} damage!");
        }

        return checkResult;
    }
}
