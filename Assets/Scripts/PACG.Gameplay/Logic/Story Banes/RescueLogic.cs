using System.Collections.Generic;

namespace PACG.Gameplay
{
    public class RescueLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        
        public RescueLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            return base.GetAvailableCardActions(card);
        }

        public override void OnDefeated(CardInstance card)
        {
            base.OnDefeated(card);
        }
    }
}
