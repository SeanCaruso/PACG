using System.Linq;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class Turn_StartTurnProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameServices _gameServices;

        public Turn_StartTurnProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameServices = gameServices;
        }

        protected override void OnExecute()
        {
            var hourCard = _contexts.GameContext?.HourDeck.DrawCard();
            _contexts.TurnContext.HourCard = hourCard;
            GameEvents.RaiseHourChanged(hourCard); // Display the Hour in the UI.

            // TODO: Handle hour powers.

            // TODO: Handle start-of-turn effects.
            var pc = _contexts.TurnContext.Character;
            if (pc.ActiveScourges.Contains(ScourgeType.Wounded))
                ScourgeRules.HandleWoundedDeckDiscard(pc, _gameServices);

            // Set initial availability of turn actions
            _contexts.TurnContext.CanGive = _contexts.TurnContext.Character.LocalCharacters.Count > 1;
            _contexts.TurnContext.CanMove = _contexts.GameContext?.Locations.Count > 1;
            _contexts.TurnContext.CanFreelyExplore = _contexts.TurnPcLocation?.Count > 0;
            _contexts.TurnContext.CanCloseLocation = _contexts.TurnPcLocation?.Count == 0;
            
            if (pc.ActiveScourges.Contains(ScourgeType.Entangled))
                _contexts.TurnContext.CanMove = false;
            
            if (pc.ActiveScourges.Contains(ScourgeType.Exhausted))
                ScourgeRules.PromptForExhaustedRemoval(pc, _gameServices);

            GameEvents.RaiseTurnStateChanged(); // Update turn action button states.

            _contexts.TurnContext.CurrentPhase = TurnPhase.TurnActions;
        }
    }
}
