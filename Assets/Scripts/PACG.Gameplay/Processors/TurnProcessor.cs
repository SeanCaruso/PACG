
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class TurnProcessor
    {
        private Deck locationDeck = new();

        // ==== RETRIEVABLE PROPERTIES =============================
        public PlayerCharacter CurrentPC => _contexts.TurnContext.CurrentPC;

        // ==== DEPENDENCIES SET VIA DEPENDENCY INJECTION IN THE CONSTRUCTOR ========================
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public TurnProcessor(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void StartTurn(PlayerCharacter pc, Deck locationDeck)
        {
            this.locationDeck = locationDeck;
            var processor = new StartTurnProcessor(pc, locationDeck, _contexts);
            _gameFlow.QueueProcessor(processor);
            _gameFlow.Process(); // Kick off the queue
        }

        public void Explore()
        {
            var processor = new ExploreProcessor(_gameServices);
            _gameFlow.QueueProcessor(processor);
            _gameFlow.Process(); // Kick off the queue
        }

        public void EndTurn()
        {
            _contexts.EndTurn();
        }

        public void OnEndTurnClicked()
        {

        }
    }
}
