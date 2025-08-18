using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class ContextManager
    {
        private ActionStagingManager _asm;

        public void Initialize(GameServices gameServices)
        {
            _asm = gameServices.ASM;
        }

        // ======================================================================
        // THE CONTEXTS
        // ======================================================================
        public GameContext GameContext { get; private set; }
        public TurnContext TurnContext { get; private set; }
        public EncounterContext EncounterContext { get; private set; }
        public CheckContext CheckContext { get; private set; }
        public IResolvable CurrentResolvable { get; private set; }

        // ======================================================================
        // STARTING / ENDING CONTEXTS
        // ======================================================================

        public void NewGame(GameContext gameContext) => GameContext = gameContext;

        public void NewTurn(TurnContext turnContext) => TurnContext = turnContext;

        public void EndTurn()
        {
            if (CheckContext != null)
                Debug.LogWarning($"[{GetType().Name}] Ending turn with a CheckContext still active!");
            if (EncounterContext != null)
                Debug.LogWarning($"[{GetType().Name}] Ending turn with an EncounterContext still active!");

            TurnContext = null;
        }

        // TODO: Think about how this will work in nested encounters - maybe use a stack?
        public void NewEncounter(EncounterContext encounterContext)
        {
            EncounterContext = encounterContext;

            encounterContext.ExploreEffects.AddRange(TurnContext.ExploreEffects);
            TurnContext.ExploreEffects.Clear();
        }

        public void EndEncounter()
        {
            GameEvents.RaiseEncounterEnded();
            EncounterContext = null;
        }

        /// <summary>
        /// USE ONLY IF YOU KNOW WHAT YOU'RE DOING! NewResolvableProcessor IS PROBABLY BETTER!
        /// </summary>
        /// <param name="resolvable"></param>
        public void NewResolvable(IResolvable resolvable)
        {
            if (CurrentResolvable != null)
                Debug.LogWarning($"[ContextManager] Created {resolvable} is overwriting {CurrentResolvable}!");

            CurrentResolvable = resolvable;

            // Automatic context creation based on resolvable type.
            if (CurrentResolvable is ICheckResolvable checkResolvable)
            {
                CheckContext = new CheckContext(checkResolvable);
                CheckContext.ExploreEffects.AddRange(EncounterContext.ExploreEffects);

                EncounterContext.ExploreEffects.RemoveAll(effect =>
                    effect is SkillBonusExploreEffect { IsForOneCheck: true });
            }

            // Now that it's set as our current resolvable and we have a CheckContext if needed,
            // do any post-construction setup.
            resolvable.Initialize();

            // Update the ActionStagingManager in case we need to show a Skip button.
            _asm.UpdateActionButtonState();
        }

        /// <summary>
        /// THIS SHOULD ONLY BE CALLED BY ActionStagingManager!
        /// </summary>
        public void EndResolvable()
        {
            CurrentResolvable?.Resolve();
            CurrentResolvable = null;
        }

        public void EndCheck() => CheckContext = null;

        // ======================================================================
        // CONVENIENCE FUNCTIONS
        // ======================================================================

        // Get relevant locations
        public Location TurnPcLocation => GameContext.GetPcLocation(TurnContext.Character);
        public Location EncounterPcLocation => GameContext.GetPcLocation(EncounterContext.Character);

        // Test for additional explorations
        public bool IsExplorePossible => (      // Exploration is possible if...
                CurrentResolvable == null &&    // ... we don't have a resolvable...
                EncounterContext == null &&     // ... or encounter, ...
                TurnPcLocation.Count > 0 &&     // ... we have more cards in the location, ...
                _asm.StagedCards.Count == 0     // ... and we don't have any currently staged cards.
            );
    }
}
