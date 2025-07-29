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

        EncounterContext context = Game.EncounterContext;

        logicRegistry = ServiceLocator.Get<LogicRegistry>();
        encounterLogic = logicRegistry.GetEncounterLogic(context.EncounteredCardData.cardID);

        Game.NewCheck(new(context.EncounterPC, CheckCategory.Combat, new(){ PF.Skill.Strength, PF.Skill.Melee }));
        foreach (EncounterPhase phase in encounterFlow)
        {
            var resolvables = encounterLogic?.Execute(phase) ?? new();

            // Resolve resolvables.
            if (resolvables.Count > 0 && resolvables[0] is CombatResolvable)
            {
                CombatResolvable combatResolvable = resolvables[0] as CombatResolvable;
                Game.NewResolution(new(combatResolvable));
                yield return Game.ResolutionContext.WaitForResolution();
                Game.EndResolution();

                yield return ResolveCombatCheck(combatResolvable.Difficulty);
            }
        }
        Game.EndCheck();

        yield break;
    }

    private IEnumerator ResolveCombatCheck(int dc)
    {
        CheckContext context = Game.CheckContext;

        // Add blessing dice.
        context.DicePool.AddDice(context.BlessingCount, Game.TurnContext.CurrentPC.GetSkill(context.UsedSkill).die);

        context.CheckPhase = CheckPhase.RollDice;
        int rollTotal = context.DicePool.Roll();
        context.CheckResult = new(rollTotal, dc, Game.TurnContext.CurrentPC, context.UsedSkill, context.Traits);

        // See if we need to prompt for rerolls.
        bool skippedReroll = false;
        while (context.CheckResult.MarginOfSuccess < Game.EncounterContext.EncounteredCardData.rerollThreshold && !skippedReroll)
        {
            bool promptReroll = false;
            var cardsToCheck = Game.TurnContext.CurrentPC.hand;
            cardsToCheck.AddRange(Game.TurnContext.CurrentPC.displayedCards);
            foreach (var card in cardsToCheck)
            {
                if (logicRegistry.GetPlayableLogic(card).GetAvailableActions().Count > 0)
                {
                    promptReroll = true;
                    break;
                }
            }

            // No playable cards allow rerolls... check if a played card set the context.
            promptReroll |= ((List<CardData>)Game.CheckContext.ContextData.GetValueOrDefault("rerollCardData", new List<CardData>())).Count > 0;

            if (promptReroll)
            {
                Debug.Log("Prompting for reroll.");
                // Reroll Resolvable
                RerollResolvable rerollResolvable = new(Game.TurnContext.CurrentPC);
                Game.NewResolution(new(rerollResolvable));
                yield return Game.ResolutionContext.WaitForResolution();
                Game.EndResolution();

                if (Game.CheckContext.ContextData.TryGetValue("doReroll", out var doReroll) && (bool)(doReroll))
                {
                    context.CheckResult.FinalRollTotal = context.DicePool.Roll();
                }
                else
                {
                    // We skipped - no more rerolls.
                    skippedReroll = true;
                    ((List<CardData>)Game.CheckContext.ContextData.GetValueOrDefault("rerollCardData", new List<CardData>())).Clear();
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

            DamageResolvable damageResolvable = new(Game.TurnContext.CurrentPC, -context.CheckResult.MarginOfSuccess);
            Game.NewResolution(new(damageResolvable));
            Debug.Log($"Rolled {context.CheckResult.FinalRollTotal} vs. {dc} - Take {damageResolvable.Amount} damage!");
            yield return Game.ResolutionContext.WaitForResolution();
            Game.EndResolution();
        }

        yield break;
    }
}
