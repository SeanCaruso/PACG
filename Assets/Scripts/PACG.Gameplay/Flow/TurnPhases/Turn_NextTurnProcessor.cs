using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    /// <summary>
    /// Processor for any automatic stuff that happens when transitioning from one turn to the next.
    /// </summary>
    public class Turn_NextTurnProcessor : BaseProcessor
    {
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public Turn_NextTurnProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        protected override void OnExecute()
        {
            // Draw up to the current player's hand size.
            _contexts.TurnContext.CurrentPC.DrawToHandSize();

            // TODO: Get next player
            var nextPc = _contexts.TurnContext.CurrentPC;
            var locationDeck = _contexts.TurnContext.LocationDeck;

            // End the current turn.
            _contexts.EndTurn();
            _gameFlow.QueueNextProcessor(new StartTurnController(nextPc, locationDeck, _gameServices));
        }
    }
}
