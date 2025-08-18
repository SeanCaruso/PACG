using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PACG.Gameplay
{
    public abstract class CardLogicBase : ILogicBase
    {
        // Dependency injection of services
        private readonly ContextManager _contexts;

        protected CardLogicBase(GameServices gameServices)
        {
            _contexts = gameServices.Contexts;
        }

        // Playable card methods (default implementations for non-playable cards)
        public List<IStagedAction> GetAvailableActions(CardInstance card)
        {
            // Only cards in hand (including reveals) and display are playable by default.
            // Resolvables will add extra actions if necessary.
            if (card.CurrentLocation is not (CardLocation.Displayed or CardLocation.Hand or CardLocation.Revealed))
                return new List<IStagedAction>();

            // If the card has any prohibited traits, (e.g., 2-Handed vs. Offhand), just return.
            foreach (var ((character, _), prohibitedTraits) in
                     _contexts.EncounterContext?.ProhibitedTraits ??
                     new Dictionary<(PlayerCharacter, CardInstance), List<string>>())
            {
                if (character == card.Owner && card.Data.traits.Intersect(prohibitedTraits).Any())
                    return new List<IStagedAction>();
            }

            return GetAvailableCardActions(card);
        }

        public virtual void OnStage(CardInstance card, IStagedAction action) { }
        public virtual void OnUndo(CardInstance card, IStagedAction action) { }
        public virtual void Execute(CardInstance card, IStagedAction action) { }

        protected virtual List<IStagedAction> GetAvailableCardActions(CardInstance card) { return new(); }

        // Encounter card methods (default implementations for playable cards)
        public virtual List<IResolvable> GetOnEncounterResolvables(CardInstance card) => new();
        public virtual List<IResolvable> GetBeforeActingResolvables(CardInstance card) => new();
        public virtual List<IResolvable> GetCheckResolvables(CardInstance card)
        {
            // Build up the list in reverse order due to LIFO processing of resolvables.
            var resolvables = new List<IResolvable>();
            foreach (var check in card.Data.checkRequirement.checkSteps)
            {
                if (check.category == CheckCategory.Combat)
                {
                    resolvables.Insert(0, new CombatResolvable(
                        _contexts.TurnContext.Character,
                        CardUtils.GetDc(check.baseDC, check.adventureLevelMult))
                    );
                }
                else
                    Debug.LogWarning($"[CardLogicBase] {card.Data.cardName} has a skill check that isn't implemented yet!");
            }
            return resolvables;
        }
    }
}
