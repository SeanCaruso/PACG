
namespace PACG.Gameplay
{
    public class Turn_RecoveryProcessor : BaseProcessor
    {
        private readonly CardManager _cardManager;
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        
        public Turn_RecoveryProcessor(GameServices gameServices) : base(gameServices)
        {
            _cardManager = gameServices.Cards;
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
        }

        protected override void OnExecute()
        {
            if (_contexts.TurnContext == null) return;
            _contexts.TurnContext.CurrentPhase = TurnPhase.Recovery;
            
            var recoveryCards = _cardManager.GetCardsInLocation(CardLocation.Recovery);
            if (recoveryCards.Count == 0) return;
            
            // Continue to run this processor until all recovery cards are gone.
            _gameFlow.Interrupt(this);

            var card = recoveryCards[0];
            recoveryCards.Remove(card);

            var resolvable = card.Logic.GetRecoveryResolvable(card);
            if (resolvable != null)
                _contexts.NewResolvable(resolvable);
        }
    }
}
