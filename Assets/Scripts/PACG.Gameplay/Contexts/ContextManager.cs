using UnityEngine;

namespace PACG.Gameplay
{
    public class ContextManager
    {
        private ActionStagingManager _asm;
        public void Iniitalize(GameServices gameServices)
        {
            _asm = gameServices.ASM;
        }

        // ======================================================================
        // THE CONTEXTS
        // ======================================================================
        public GameContext GameContext { get; private set; } = null;
        public TurnContext TurnContext { get; private set; } = null;
        public EncounterContext EncounterContext { get; private set; } = null;
        public CheckContext CheckContext { get; private set; } = null;
        public IResolvable CurrentResolvable { get; private set; } = null;
        // ======================================================================

        public void NewGame(GameContext gameContext) => GameContext = gameContext;

        public void NewTurn(TurnContext turnContext) => TurnContext = turnContext;
        public void EndTurn() => TurnContext = null;

        // TODO: Think about how this will work in nested encounters - maybe use a stack?
        public void NewEncounter(EncounterContext encounterContext) => EncounterContext = encounterContext;
        public void EndEncounter() => EncounterContext = null;

        /// <summary>
        /// Adds a new resolvable to pause the game for user input (and create a CheckContext if needed).
        /// </summary>
        /// <param name="resolvable"></param>
        public void NewResolvable(IResolvable resolvable)
        {
            if (CurrentResolvable != null) Debug.LogWarning($"[ContextManager] Created {resolvable} is overwriting {CurrentResolvable}!");

            CurrentResolvable = resolvable;

            // Automatic context creation based on resolvable type.
            if (CurrentResolvable is ICheckResolvable checkResolvable)
            {
                CheckContext = new(checkResolvable);
            }

            // Update the ActionStagingManager in case we need to show a Skip button.
            _asm.UpdateActionButtonState();
        }

        /// <summary>
        /// THIS SHOULD ONLY BE CALLED BY ActionStagingManager.Commit!
        /// </summary>
        public void EndResolution() => CurrentResolvable = null;
    }
}
