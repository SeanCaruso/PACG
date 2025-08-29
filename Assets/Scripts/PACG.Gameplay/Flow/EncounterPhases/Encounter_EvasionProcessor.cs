using System;
using System.Linq;
using PACG.SharedAPI;
using UnityEngine;

namespace PACG.Gameplay
{
    public class Encounter_EvasionProcessor : BaseProcessor
    {
        // Dependency injection
        private readonly CardManager _cardManager;
        private readonly ContextManager _contexts;
        
        public Encounter_EvasionProcessor(GameServices gameServices) : base(gameServices)
        {
            _cardManager = gameServices.Cards;
            _contexts = gameServices.Contexts;
        }

        protected override void OnExecute()
        {
            if (_contexts.EncounterContext == null) return;

            if (_contexts.EncounterContext.Card?.Logic?.CanEvade == false) return;
            
            // The Entangled scourge prevents evasion.
            if (_contexts.EncounterContext.Character.ActiveScourges.Contains(ScourgeType.Entangled)) return;
            
            // First, see if anything added an evasion effect to this exploration.
            if (_contexts.EncounterContext.ExploreEffects.Any(effect => effect is EvadeExploreEffect))
            {
                PromptForEvasion(EvadeEncounter);
                return;
            }
            
            // If not, update the phase and check for available evasion powers.
            _contexts.EncounterContext.CurrentPhase = EncounterPhase.Evasion;
            
            // TODO: Check character powers.

            // Finally, check characters' cards.
            if (!_cardManager.FindAll(c => c.Owner != null && c.GetAvailableActions().Any()).Any())
                return;
            
            GameEvents.SetStatusText("Evade?");
            
            _contexts.NewResolvable(new EvadeResolvable(EvadeEncounter));
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
            if (_contexts.EncounterContext == null) return;
            
            Debug.Log($"[{GetType().Name}] Evading {_contexts.EncounterContext.Card}");
            
            if (_contexts.EncounterContext.Card.CurrentLocation == CardLocation.Deck)
                _contexts.EncounterContext.Character.Location.ShuffleIn(_contexts.EncounterContext.Card, true);
            
            // Null out the encounter for the other Encounter sub-processors.
            _contexts.EndEncounter();
        }
    }
}
