
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class StartTurnController : IProcessor, IPhaseController
    {
        private readonly PlayerCharacter _pc;
        private readonly Deck _locationDeck;

        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        private static int _turnNumber = 0;

        public StartTurnController(PlayerCharacter pc, Deck locationDeck, GameServices gameServices)
        {
            _pc = pc;
            _locationDeck = locationDeck;

            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void Execute()
        {
            Debug.Log($"===== STARTING TURN {_turnNumber++} =====");

            _contexts.NewTurn(new(_pc, _locationDeck));
            GameEvents.RaisePlayerCharacterChanged(_pc);

            _gameFlow.QueueNextProcessor(new Turn_StartTurnProcessor(_gameServices));

            _gameFlow.CompleteCurrentPhase();
        }
    }
}
