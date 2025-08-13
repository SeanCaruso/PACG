
namespace PACG.Gameplay
{
    public class EndTurnController : IProcessor, IPhaseController
    {
        private readonly bool _skipOptionalDiscards;

        private readonly CardManager _cardManager;
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public EndTurnController(bool skipOptionalDiscards, GameServices gameServices)
        {
            _skipOptionalDiscards = skipOptionalDiscards;

            _cardManager = gameServices.Cards;
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void Execute()
        {
            _gameFlow.QueueNextProcessor(new Turn_EndOfTurnProcessor(_gameServices));

            if (_cardManager.GetCardsInLocation(CardLocation.Recovery).Count > 0)
                _gameFlow.QueueNextProcessor(new Turn_RecoveryProcessor(_gameServices));

            var pc = _contexts.TurnContext.Character;
            if (!_skipOptionalDiscards || _cardManager.GetCardsInHand(pc).Count > pc.CharacterData.handSize)
                _gameFlow.QueueNextProcessor(new Turn_DiscardDuringResetProcessor(_gameServices));

            _gameFlow.QueueNextProcessor(new Turn_NextTurnProcessor(_gameServices));

            _gameFlow.CompleteCurrentPhase();
        }
    }
}
