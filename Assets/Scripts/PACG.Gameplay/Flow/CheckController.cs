
namespace PACG.Gameplay
{
    public class CheckController : IProcessor, IPhaseController
    {
        private readonly CheckResolvable _resolvable;

        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public CheckController(CheckResolvable resolvable, GameServices gameServices)
        {
            _resolvable = resolvable;

            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public void Execute()
        {
            // CheckContext creation is handled by GameFlowManager calling NewResolution.

            _gameFlow.QueueNextProcessor(new Check_RollDiceProcessor(_gameServices));

            if (_resolvable.Card is CardInstance card && card.Data.cardType == PF.CardType.Monster)
            {
                _gameFlow.QueueNextProcessor(new Check_DamageProcessor(_gameServices));
            }

            _gameFlow.QueueNextProcessor(new Check_EndCheckProcessor(_gameServices));

            _gameFlow.CompleteCurrentPhase();
        }
    }
}
