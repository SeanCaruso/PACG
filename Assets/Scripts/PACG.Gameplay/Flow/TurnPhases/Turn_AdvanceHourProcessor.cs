using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class Turn_AdvanceHourProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly ContextManager _contexts;

        public Turn_AdvanceHourProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            if (_contexts.TurnContext == null) return;
            _contexts.TurnContext.CurrentPhase = TurnPhase.TurnStart;

            var hourCard = _contexts.GameContext?.HourDeck.DrawCard();
            _contexts.TurnContext.HourCard = hourCard;
            GameEvents.RaiseHourChanged(hourCard); // Display the Hour in the UI.

            // TODO: Handle hour powers.
        }
    }
}
