using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class StartTurnProcessor : IProcessor
    {
        private readonly PlayerCharacter _pc;
        private readonly Deck _locationDeck;
        private readonly ContextManager _contexts;

        public StartTurnProcessor(PlayerCharacter pc, Deck locationDeck, ContextManager contexts)
        {
            _pc = pc;
            _locationDeck = locationDeck;
            _contexts = contexts;
        }

        public void Execute()
        {
            var hourCard = _contexts.GameContext.HourDeck.DrawCard();
            GameEvents.RaiseHourChanged(hourCard);

            _contexts.NewTurn(new(hourCard, _pc, _locationDeck));

            // Set initial availability of turn actions
            _contexts.TurnContext.CanGive = true; // TODO: Implement logic after we have multiple characters.
            _contexts.TurnContext.CanMove = true; // TODO: Implement logic after we have multiple locations
            _contexts.TurnContext.CanExplore = _locationDeck.Count > 0;
            _contexts.TurnContext.CanCloseLocation = _locationDeck.Count == 0;

            GameEvents.RaiseTurnStateChanged();
        }
    }
}
