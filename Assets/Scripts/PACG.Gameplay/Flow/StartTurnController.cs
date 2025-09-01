using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class StartTurnController : IProcessor, IPhaseController
    {
        private readonly PlayerCharacter _pc;

        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public StartTurnController(PlayerCharacter pc, GameServices gameServices)
        {
            _pc = pc;

            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void Execute()
        {
            Debug.Log($"===== STARTING TURN {_contexts.GameContext.TurnNumber++} =====");

            _contexts.NewTurn(new TurnContext(_pc));
            GameEvents.RaisePlayerCharacterChanged(_pc);
            GameEvents.RaiseLocationChanged(_pc, _pc.Location);

            _gameFlow.QueueNextProcessor(new Turn_AdvanceHourProcessor(_gameServices));
            _gameFlow.QueueNextProcessor(new Turn_StartTurnProcessor(_gameServices));

            _gameFlow.CompleteCurrentPhase();
        }
    }
}
