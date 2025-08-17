using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Encounter_EvasionProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        
        public Encounter_EvasionProcessor(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            // TODO: Get evasion powers on cards.

            // If we have no evasion explore effects, just continue on our merry way.
            if (!_contexts.EncounterContext.ExploreEffects.Any(effect => effect is EvadeExploreEffect)) return;
            
            var resolvable = new PlayerChoiceResolvable("Evade?",
                new PlayerChoiceResolvable.ChoiceOption("Evade", EvadeEncounter),
                new PlayerChoiceResolvable.ChoiceOption("Encounter", () => { })
            );
            _contexts.NewResolvable(resolvable);
        }

        private void EvadeEncounter()
        {
            Debug.Log($"[{GetType().Name}] Evading {_contexts.EncounterContext.Card}");
            
            if (_contexts.EncounterContext.Card.CurrentLocation == CardLocation.Deck)
                _contexts.TurnContext.Location.ShuffleIn(_contexts.EncounterContext.Card, true);
            
            _contexts.EndEncounter();
        }
    }
}
