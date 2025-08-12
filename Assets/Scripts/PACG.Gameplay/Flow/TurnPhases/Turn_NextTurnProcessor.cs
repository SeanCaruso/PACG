using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
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
            // TODO: Get next player
            var nextPc = _contexts.TurnContext.CurrentPC;
            var locationDeck = _contexts.TurnContext.LocationDeck;

            // End the current turn.
            _contexts.EndTurn();
            _gameFlow.QueueNextProcessor(new TurnController(nextPc, locationDeck, _gameServices));
        }
    }
}
