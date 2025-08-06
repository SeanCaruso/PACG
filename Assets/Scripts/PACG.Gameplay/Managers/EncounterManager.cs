
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PACG.Gameplay
{
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

    public class EncounterManager
    {
        // Populated via dependency injection
        private readonly LogicRegistry _logic;
        private readonly ContextManager _contexts;
        private readonly ActionStagingManager _actionStagingManager;

        private IEncounterLogic encounterLogic = null;

        public EncounterManager(LogicRegistry logicRegistry, ContextManager contextManager, ActionStagingManager actionStagingManager)
        {
            _logic = logicRegistry;
            _contexts = contextManager;
            _actionStagingManager = actionStagingManager;
        }

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

            EncounterContext context = _contexts.EncounterContext;

            encounterLogic = _logic.GetEncounterLogic(context.EncounteredCard);

            _contexts.NewCheck(new(context.EncounterPC, context.EncounteredCard.Data.checkRequirement.checkSteps[0], _contexts.GameContext));
            foreach (EncounterPhase phase in encounterFlow)
            {
                var resolvables = encounterLogic?.Execute(phase) ?? new();

                // Resolve resolvables.
                if (resolvables.Count > 0 && resolvables[0] is CombatResolvable)
                {
                    CombatResolvable combatResolvable = resolvables[0] as CombatResolvable;
                    _contexts.NewResolution(new(combatResolvable), _actionStagingManager);
                    yield return _contexts.ResolutionContext.WaitForResolution();
                    _contexts.EndResolution();

                    yield return ResolveCombatCheck(combatResolvable.Difficulty);
                }
            }
            _contexts.EndCheck();

            yield break;
        }

        private IEnumerator ResolveCombatCheck(int dc)
        {
            CheckContext context = _contexts.CheckContext;

            // Add blessing dice.
            context.DicePool.AddDice(context.BlessingCount, _contexts.TurnContext.CurrentPC.GetSkill(context.UsedSkill).die);

            context.CheckPhase = CheckPhase.RollDice;
            int rollTotal = context.DicePool.Roll();
            context.CheckResult = new(rollTotal, dc, _contexts.TurnContext.CurrentPC, context.UsedSkill, context.Traits);

            // See if we need to prompt for rerolls.
            bool skippedReroll = false;
            while (context.CheckResult.MarginOfSuccess < _contexts.EncounterContext.EncounteredCard.Data.rerollThreshold && !skippedReroll)
            {
                bool promptReroll = false;
                var cardsToCheck = _contexts.TurnContext.CurrentPC.Hand.Union(_contexts.TurnContext.CurrentPC.DisplayedCards);
                foreach (var card in cardsToCheck)
                {
                    if (_logic.GetPlayableLogic(card).GetAvailableActions().Count > 0)
                    {
                        promptReroll = true;
                        break;
                    }
                }

                // No playable cards allow rerolls... check if a played card set the context.
                promptReroll |= ((List<CardInstance>)_contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Count > 0;

                if (promptReroll)
                {
                    Debug.Log("Prompting for reroll.");
                    // Reroll Resolvable
                    RerollResolvable rerollResolvable = new(_logic, _contexts.TurnContext.CurrentPC, _contexts.CheckContext);
                    _contexts.NewResolution(new(rerollResolvable), _actionStagingManager);
                    yield return _contexts.ResolutionContext.WaitForResolution();
                    _contexts.EndResolution();

                    if (_contexts.CheckContext.ContextData.TryGetValue("doReroll", out var doReroll) && (bool)(doReroll))
                    {
                        context.CheckResult.FinalRollTotal = context.DicePool.Roll();
                    }
                    else
                    {
                        // We skipped - no more rerolls.
                        skippedReroll = true;
                        ((List<CardInstance>)_contexts.CheckContext.ContextData.GetValueOrDefault("rerollCards", new List<CardInstance>())).Clear();
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

                DamageResolvable damageResolvable = new(_logic, _contexts.TurnContext.CurrentPC, -context.CheckResult.MarginOfSuccess);
                _contexts.NewResolution(new(damageResolvable), _actionStagingManager);
                Debug.Log($"Rolled {context.CheckResult.FinalRollTotal} vs. {dc} - Take {damageResolvable.Amount} damage!");
                yield return _contexts.ResolutionContext.WaitForResolution();
                _contexts.EndResolution();
            }

            yield break;
        }
    }
}
