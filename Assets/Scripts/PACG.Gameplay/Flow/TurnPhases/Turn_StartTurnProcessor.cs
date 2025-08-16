using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class Turn_StartTurnProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;

        public Turn_StartTurnProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            var hourCard = _contexts.GameContext.HourDeck.DrawCard();
            _contexts.TurnContext.HourCard = hourCard;
            GameEvents.RaiseHourChanged(hourCard); // Display the Hour in the UI.

            // TODO: Handle hour powers.

            // TODO: Handle start-of-turn effects.

            // Set initial availability of turn actions
            _contexts.TurnContext.CanGive = _contexts.TurnContext.Character.LocalCharacters.Count > 0;
            _contexts.TurnContext.CanMove = _contexts.GameContext.Locations.Count > 1;
            _contexts.TurnContext.CanFreelyExplore = _contexts.TurnPcLocation.Count > 0;
            _contexts.TurnContext.CanCloseLocation = _contexts.TurnPcLocation.Count == 0;

            GameEvents.RaiseTurnStateChanged(); // Update turn action button states.

            _contexts.TurnContext.CurrentPhase = TurnPhase.TurnActions;
        }
    }
}
