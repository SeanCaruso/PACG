using System.Linq;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class Turn_StartTurnProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly CardManager _cardManager;
        private readonly ContextManager _contexts;

        public Turn_StartTurnProcessor(GameServices gameServices) : base(gameServices)
        {
            _cardManager = gameServices.Cards;
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            var hourCard = _contexts.GameContext.HourDeck.DrawCard();
            _contexts.TurnContext.HourCard = hourCard;
            GameEvents.RaiseHourChanged(hourCard); // Display the Hour in the UI.

            // TODO: Handle hour powers.

            // TODO: Handle start-of-turn effects.
            var pc = _contexts.TurnContext.Character;
            if (pc.ActiveScourges.Contains(ScourgeType.Wounded))
                _cardManager.MoveCard(pc.Deck.DrawCard(), PF.ActionType.Discard);

            // Set initial availability of turn actions
            _contexts.TurnContext.CanGive = _contexts.TurnContext.Character.LocalCharacters.Count > 0;
            _contexts.TurnContext.CanMove = _contexts.GameContext.Locations.Count > 1;
            _contexts.TurnContext.CanFreelyExplore = _contexts.TurnPcLocation?.Count > 0;
            _contexts.TurnContext.CanCloseLocation = _contexts.TurnPcLocation?.Count == 0;
            
            if (pc.ActiveScourges.Contains(ScourgeType.Entangled))
                _contexts.TurnContext.CanMove = false;

            GameEvents.RaiseTurnStateChanged(); // Update turn action button states.

            _contexts.TurnContext.CurrentPhase = TurnPhase.TurnActions;
        }
    }
}
