using PACG.Data;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class Turn_EndOfTurnProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public Turn_EndOfTurnProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        protected override void OnExecute()
        {
            var locationPower = _contexts.TurnPcLocation.GetEndOfTurnPower();
            
            CharacterPower? characterPower = null;
            if (!_contexts.TurnContext.ForceEndTurn)
                characterPower = _contexts.TurnContext.Character.GetEndOfTurnPower();

            if (locationPower == null && characterPower == null)
                return;
            
            // We'll need to process this again in case there are more valid powers.
            _gameFlow.Interrupt(this);
            
            GameEvents.SetStatusText("Use End-of-Turn Power?");

            var resolvable = new PowersAvailableResolvable(locationPower, characterPower, _gameServices);
            var processor = new NewResolvableProcessor(resolvable, _gameServices);
            _gameFlow.StartPhase(processor, "End-of-Turn");
        }
    }
}
