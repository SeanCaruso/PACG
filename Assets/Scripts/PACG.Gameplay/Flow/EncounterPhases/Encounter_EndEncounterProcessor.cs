using System.Linq;
using PACG.SharedAPI;

namespace PACG.Gameplay
{
    public class Encounter_EndEncounterProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public Encounter_EndEncounterProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
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
            
            GameEvents.RaiseEncounterEnded();

            if (wasSuccess && IsClosingHenchman(encounteredCard))
            {
                var pc = _contexts.EncounterContext.Character;

                var closeChoiceResolvable = new PlayerChoiceResolvable("Close location?",
                    new PlayerChoiceResolvable.ChoiceOption("Close", () =>
                    {
                        var closeResolvable = pc.Location.GetToCloseResolvable();
                        var closeProcessor = new NewResolvableProcessor(closeResolvable, _gameServices);
                        _gameFlow.Interrupt(closeProcessor);
                    }),
                    new PlayerChoiceResolvable.ChoiceOption("Skip", () => { }));
                
                var nextProcessor = new NewResolvableProcessor(closeChoiceResolvable, _gameServices);
                _gameFlow.Interrupt(nextProcessor);
            }

            _contexts.EndEncounter();
        }

        private bool IsClosingHenchman(CardInstance card)
        {
            if (card == null) return false;

            return _contexts.GameContext?.ScenarioData?.Henchmen.Any(henchman =>
                henchman.CardData.cardID == card.Data.cardID && henchman.IsClosing) == true;
        }
    }
}
