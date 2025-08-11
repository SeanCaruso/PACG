
namespace PACG.Gameplay
{
    public class CheckController : IProcessor, IPhaseController
    {
        private readonly ICheckResolvable _resolvable;

        private readonly GameServices _gameServices;
        private GameFlowManager GFM => _gameServices.GameFlow;

        public CheckController(ICheckResolvable resolvable, GameServices gameServices)
        {
            _resolvable = resolvable;

            _gameServices = gameServices;
        }

        public void Execute()
        {
            // CheckContext creation is handled by GameFlowManager calling NewResolution.

            GFM.QueueNextProcessor(new Check_RollDiceProcessor(_gameServices));

            if (_resolvable is CombatResolvable)
            {
                GFM.QueueNextProcessor(new Check_DamageProcessor(_gameServices));
            }

            GFM.CompleteCurrentPhase();
        }
    }
}
