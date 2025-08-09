using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        // Playable card methods (default implementations for non-playable cards)
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

        public virtual void OnStage(IStagedAction _) { }
        public virtual void OnUndo(IStagedAction _) { }
        public virtual void Execute(IStagedAction _) { }

        protected virtual List<IStagedAction> GetAvailableCardActions() { return new(); }

        // Encounter card methods (default implementations for playable cards)
        public virtual List<IResolvable> GetOnEncounterResolvables() => new();
        public virtual List<IResolvable> GetBeforeActingResolvables() => new();
        public virtual List<IResolvable> GetCheckResolvables()
        {
            // Build up the list in reverse order due to LIFO processing of resolvables.
            var resolvables = new List<IResolvable>();
            foreach (var check in Card.Data.checkRequirement.checkSteps)
            {
                if (check.category == CheckCategory.Combat)
                    resolvables.Insert(0, new CombatResolvable(GameServices.Logic, GameServices.Contexts.TurnContext.CurrentPC, CardUtils.GetDC(check.baseDC, check.adventureLevelMult)));
                else
                    Debug.LogWarning($"[CardLogicBase] {Card.Data.cardName} has a skill check that isn't implemented yet!");
            }
            return resolvables;
        }
    }
}