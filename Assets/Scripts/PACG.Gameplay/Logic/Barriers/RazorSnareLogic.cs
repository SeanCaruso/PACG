namespace PACG.Gameplay
{
    public class RazorSnareLogic : CardLogicBase
    {
        // Dependency injections
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;
        
        public RazorSnareLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override void OnUndefeated(CardInstance card)
        {
            base.OnUndefeated(card);
            _contexts.EncounterContext?.Character?.AddScourge(ScourgeType.Entangled);
            _contexts.EncounterContext?.Character?.AddScourge(ScourgeType.Wounded);

            _contexts.TurnContext.ForceEndTurn = true;
            
            _gameFlow.QueueNextProcessor(new EndTurnController(false, _gameServices));
        }
    }
}
