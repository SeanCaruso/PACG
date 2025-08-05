using UnityEngine;

namespace PACG.Gameplay
{
    public class ContextManager : MonoBehaviour
    {
        public GameContext GameContext { get; private set; } = null;
        public TurnContext TurnContext { get; private set; } = null;
        public EncounterContext EncounterContext { get; private set; } = null;
        public CheckContext CheckContext { get; private set; } = null;
        public ResolutionContext ResolutionContext { get; private set; } = null;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void NewGame(GameContext gameContext) => GameContext = gameContext;

        public void NewTurn(TurnContext turnContext) => TurnContext = turnContext;
        public void EndTurn() => TurnContext = null;

        public void NewEncounter(EncounterContext encounterContext) => EncounterContext = encounterContext;
        public void EndEncounter() => EncounterContext = null;

        public void NewCheck(CheckContext checkContext) => CheckContext = checkContext;
        public void EndCheck() => CheckContext = null;

        public void NewResolution(ResolutionContext resolutionContext)
        {
            ResolutionContext = resolutionContext;

            // Update the ActionStagingManager in case we need to show a Skip button.
            ServiceLocator.Get<ActionStagingManager>().UpdateActionButtonState();
        }
        public void EndResolution() => ResolutionContext = null;
    }
}
