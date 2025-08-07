using UnityEngine;

namespace PACG.Gameplay
{
    public class ContextManager
    {
        public GameContext GameContext { get; private set; } = null;
        public TurnContext TurnContext { get; private set; } = null;
        public EncounterContext EncounterContext { get; private set; } = null;
        public CheckContext CheckContext { get; private set; } = null;
        public ResolutionContext ResolutionContext { get; private set; } = null;

        public void NewGame(GameContext gameContext) => GameContext = gameContext;

        public void NewTurn(TurnContext turnContext) => TurnContext = turnContext;
        public void EndTurn() => TurnContext = null;

        // TODO: Think about how this will work in nested encounters - maybe use a stack?
        public void NewEncounter(EncounterContext encounterContext) => EncounterContext = encounterContext;
        public void EndEncounter() => EncounterContext = null;

        public void NewResolution(ResolutionContext resolutionContext)
        {
            ResolutionContext = resolutionContext;

            // Automatic context creation based on resolvable type.
            if (resolutionContext.CurrentResolvable is ICheckResolvable checkResolvable)
            {
                CheckContext = new(checkResolvable);
            }

            // Update the ActionStagingManager in case we need to show a Skip button.
            _asm.UpdateActionButtonState();
        }
        public void EndResolution()
        {
            // Automatic context clean-up based on resolvable type.
            if (ResolutionContext.CurrentResolvable is ICheckResolvable)
            {
                CheckContext = null;
            }

            ResolutionContext = null;
        }

        private ActionStagingManager _asm;
        public void InjectActionStagingManager(ActionStagingManager asm)
        {
            _asm = asm;
        }
    }
}
