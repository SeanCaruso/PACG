using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Turn_StartTurnProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;

        public Turn_StartTurnProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
        }

        protected override void OnExecute()
        {
            var hourCard = _contexts.GameContext.HourDeck.DrawCard();
            _contexts.TurnContext.HourCard = hourCard;
            GameEvents.RaiseHourChanged(hourCard); // Display the Hour in the UI.

            // TODO: Handle hour powers.

            // TODO: Handle start-of-turn effects.

            // Set initial availability of turn actions
            _contexts.TurnContext.CanGive = false; // TODO: Implement logic after we have multiple characters.
            _contexts.TurnContext.CanMove = false; // TODO: Implement logic after we have multiple locations
            _contexts.TurnContext.CanExplore = _contexts.TurnContext.LocationDeck.Count > 0;
            _contexts.TurnContext.CanCloseLocation = _contexts.TurnContext.LocationDeck.Count == 0;

            GameEvents.RaiseTurnStateChanged(_contexts.TurnContext); // Update turn action button states.

            _contexts.TurnContext.CurrentPhase = TurnPhase.TurnActions;
        }
    }
}
