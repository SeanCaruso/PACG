namespace PACG.Gameplay
{
    public class DrowningMudLogic : CardLogicBase
    {
        // Dependency injections
        private readonly CardManager _cards;
        
        private readonly ContextManager _contexts;
        
        public DrowningMudLogic(GameServices gameServices) : base(gameServices)
        {
            _cards = gameServices.Cards;
            _contexts = gameServices.Contexts;
        }

        public override void OnUndefeated(CardInstance card)
        {
            base.OnUndefeated(card);
            _contexts.EncounterContext?.Character?.AddScourge(ScourgeType.Entangled);
            _contexts.EncounterContext?.Character?.AddScourge(ScourgeType.Exhausted);

            var topCard = _contexts.EncounterContext?.Character?.DrawFromDeck();
            _cards.MoveCard(topCard, CardLocation.Buried);
        }
    }
}
