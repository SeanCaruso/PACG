using System;
using System.Linq;
using PACG.SharedAPI;
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
            if (_contexts.EncounterContext == null) return;
            
            // First, see if anything added an evasion effect to this exploration.
            if (_contexts.EncounterContext.ExploreEffects.Any(effect => effect is EvadeExploreEffect))
            {
                PromptForEvasion(EvadeEncounter);
                return;
            }
            
            // If not, update the phase and check for available evasion powers.
            _contexts.EncounterContext.CurrentPhase = EncounterPhase.Evasion;
            
            // TODO: Check character powers.

            // Finally, check the encounter PC's cards.
            if (!_contexts.EncounterContext.Character.AllCards.Any(card => card.GetAvailableActions().Count > 0))
                return;
            
            GameEvents.SetStatusText("Evade?");
            
            _contexts.NewResolvable(new GenericResolvable(EvadeEncounter));
        }

        private void PromptForEvasion(Action onEvade)
        {
            var resolvable = new PlayerChoiceResolvable("Evade?",
                new PlayerChoiceResolvable.ChoiceOption("Evade", onEvade),
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
