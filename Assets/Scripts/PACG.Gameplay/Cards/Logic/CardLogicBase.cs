using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public abstract class CardLogicBase : ILogicBase
    {
        // Dependency injection of services
        private readonly CardManager _cards;
        private readonly ContextManager _contexts;

        protected CardLogicBase(GameServices gameServices)
        {
            _cards = gameServices.Cards;
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
            var prohibitedTraits = _contexts.EncounterContext?.ProhibitedTraits
                .GetValueOrDefault(card.Owner, new HashSet<string>()).ToList() ?? new List<string>();
            
            prohibitedTraits.AddRange(_contexts.CheckContext?.ProhibitedTraits(card.Owner) ?? new List<string>());
            if (prohibitedTraits.Intersect(card.Traits).Any())
                return new List<IStagedAction>();

            return GetAvailableCardActions(card);
        }

        /// <summary>
        /// Returns a description of how this card's action modifies a check for preview and staging purposes.
        /// This is a pure, read-only query and should have no side effects.
        /// </summary>
        public virtual CheckModifier GetCheckModifier(IStagedAction action) => null;

        /// <summary>
        /// Applies the permanent, one-time effects of an action when it is committed.
        /// This should NOT modify the dice pool.
        /// </summary>
        public virtual void OnCommit(IStagedAction action)
        {
        }

        protected virtual List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            return new List<IStagedAction>();
        }

        // Encounter card methods (default implementations for playable cards)
        public virtual IResolvable GetOnEncounterResolvable(CardInstance card) => null;
        public virtual IResolvable GetBeforeActingResolvable(CardInstance card) => null;
        public virtual IResolvable GetResolveEncounterResolvable(CardInstance card) => null;

        public virtual IResolvable GetCheckResolvable(CardInstance card)
        {
            if (card.Data.checkRequirement.mode is CheckMode.Choice or CheckMode.Single)
            {
                return new CheckResolvable(card, _contexts.EncounterContext.Character, card.Data.checkRequirement);
            }

            // TODO: Handle sequential and "None" modes.
            return null;
        }

        public virtual void OnDefeated(CardInstance card)
        {
            if (card.IsBane)
            {
                _cards.MoveCard(card, CardLocation.Vault);
            }
            else
            {
                _contexts.EncounterContext.Character.AddToHand(card);
            }
        }
        public virtual void OnUndefeated(CardInstance card)
        {
            if (card.IsBane)
            {
                _contexts.EncounterPcLocation.ShuffleIn(card, true);
            }
            else
            {
                _cards.MoveCard(card, CardLocation.Vault);
            }
        }

        // Other card methods (recovery)
        public virtual IResolvable GetRecoveryResolvable(CardInstance card) => null;
    }
}
