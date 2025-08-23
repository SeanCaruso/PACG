using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class Encounter_EndEncounterProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        
        public Encounter_EndEncounterProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            if (_contexts.EncounterContext == null) return;

            var wasSuccess = _contexts.EncounterContext.CheckResult.WasSuccess;
            var encounteredCard = _contexts.EncounterContext.Card;
            
            if (wasSuccess)
                encounteredCard.Logic.OnDefeated(encounteredCard);
            else
                encounteredCard.Logic.OnUndefeated(encounteredCard);
            
            _contexts.EndEncounter();
        }
    }
}
