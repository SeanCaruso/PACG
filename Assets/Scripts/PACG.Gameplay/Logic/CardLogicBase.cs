
using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public abstract class CardLogicBase : ICardLogic
    {
        public CardInstance Card { get; set; }

        // Dependency injection of services
        protected GameServices GameServices { get; }
        protected ContextManager Contexts { get; }
        protected LogicRegistry Logic { get; }

        protected CardLogicBase(GameServices gameServices)
        {
            GameServices = gameServices;

            Contexts = gameServices.Contexts;
            Logic = gameServices.Logic;
        }

        public virtual List<IStagedAction> GetAvailableActions()
        {
            // If the card has any prohibited traits, (e.g. 2-Handed vs. Offhand), just return.
            foreach (((var character, _), var prohibitedTraits) in Contexts.EncounterContext?.ProhibitedTraits ?? new())
            {
                if (character == Card.Owner && Card.Data.traits.Intersect(prohibitedTraits).Any())
                    return new();
            }

            return GetAvailableCardActions();
        }

        protected virtual List<IStagedAction> GetAvailableCardActions() { return new(); }

    }
}