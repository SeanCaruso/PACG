
namespace PACG.Gameplay
{
    public class TurnController : IProcessor, IPhaseController
    {
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public TurnController(PlayerCharacter pc, Deck locationDeck, GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;

            _contexts.NewTurn(new(pc, locationDeck));
        }

        public void Execute()
        {
            _gameFlow.QueueNextProcessor(new StartTurnProcessor(_gameServices));

            _gameFlow.CompleteCurrentPhase();
        }
    }
}
