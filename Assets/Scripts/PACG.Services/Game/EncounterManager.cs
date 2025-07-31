using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class EncounterManager : GameBehaviour
{
    private LogicRegistry logicRegistry = null;

    private IEncounterLogic encounterLogic = null;

    public IEnumerator RunEncounter()
    {
        List<EncounterPhase> encounterFlow = new()
        {
            EncounterPhase.OnEncounter,
            EncounterPhase.Evasion,
            EncounterPhase.BeforeActing,
            EncounterPhase.AttemptChecks,
            EncounterPhase.AfterActing,
            EncounterPhase.Resolve,
            EncounterPhase.Avenge,
        };

        EncounterContext context = Contexts.EncounterContext;

        logicRegistry = ServiceLocator.Get<LogicRegistry>();
        encounterLogic = logicRegistry.GetEncounterLogic(context.EncounteredCard);

        Contexts.NewCheck(new(context.EncounterPC, context.EncounteredCard.Data.checkRequirement.checkSteps[0], Contexts.GameContext));
        foreach (EncounterPhase phase in encounterFlow)
        {
            var resolvables = encounterLogic?.Execute(phase) ?? new();

            // Resolve resolvables.
            if (resolvables.Count > 0 && resolvables[0] is CombatResolvable)
            {
                CombatResolvable combatResolvable = resolvables[0] as CombatResolvable;
                Contexts.NewResolution(new(combatResolvable));
                yield return Contexts.ResolutionContext.WaitForResolution();
                Contexts.EndResolution();

                yield return ResolveCombatCheck(combatResolvable.Difficulty);
            }
        }
        Contexts.EndCheck();

        yield break;
    }

    private IEnumerator ResolveCombatCheck(int dc)
    {
        CheckContext context = Contexts.CheckContext;

        // Add blessing dice.
        context.DicePool.AddDice(context.BlessingCount, Contexts.TurnContext.CurrentPC.GetSkill(context.UsedSkill).die);

        context.CheckPhase = CheckPhase.RollDice;
        int rollTotal = context.DicePool.Roll();
        context.CheckResult = new(rollTotal, dc, Contexts.TurnContext.CurrentPC, context.UsedSkill, context.Traits);

        // See if we need to prompt for rerolls.
        bool skippedReroll = false;
        while (context.CheckResult.MarginOfSuccess < Contexts.EncounterContext.EncounteredCard.Data.rerollThreshold && !skippedReroll)
        {
            bool promptReroll = false;
            var cardsToCheck = Contexts.TurnContext.CurrentPC.Hand.Union(Contexts.TurnContext.CurrentPC.DisplayedCards);
            foreach (var card in cardsToCheck)
            {
                if (logicRegistry.GetPlayableLogic(card).GetAvailableActions().Count > 0)
                {
                    promptReroll = true;
                    break;
                }
            }

            // No playable cards allow rerolls... check if a played card set the context.
            promptReroll |= ((List<CardInstance>)Contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Count > 0;

            if (promptReroll)
            {
                Debug.Log("Prompting for reroll.");
                // Reroll Resolvable
                RerollResolvable rerollResolvable = new(Contexts.TurnContext.CurrentPC);
                Contexts.NewResolution(new(rerollResolvable));
                yield return Contexts.ResolutionContext.WaitForResolution();
                Contexts.EndResolution();

                if (Contexts.CheckContext.ContextData.TryGetValue("doReroll", out var doReroll) && (bool)(doReroll))
                {
                    context.CheckResult.FinalRollTotal = context.DicePool.Roll();
                }
                else
                {
                    // We skipped - no more rerolls.
                    skippedReroll = true;
                    ((List<CardInstance>)Contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Clear();
                }
            }
            else
            {
                Debug.Log("No reroll options.");
                break;
            }
        }

        if (context.CheckResult.WasSuccess)
        {
            Debug.Log($"Rolled {context.CheckResult.FinalRollTotal} vs. {dc} - Success!");
        }
        else if (false /* avenge? */)
        { }
        else
        {
            context.CheckPhase = CheckPhase.SufferDamage;

            DamageResolvable damageResolvable = new(Contexts.TurnContext.CurrentPC, -context.CheckResult.MarginOfSuccess);
            Contexts.NewResolution(new(damageResolvable));
            Debug.Log($"Rolled {context.CheckResult.FinalRollTotal} vs. {dc} - Take {damageResolvable.Amount} damage!");
            yield return Contexts.ResolutionContext.WaitForResolution();
            Contexts.EndResolution();
        }

        yield break;
    }
}
