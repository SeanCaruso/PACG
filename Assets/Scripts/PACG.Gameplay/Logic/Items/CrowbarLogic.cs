using System.Collections.Generic;
using PACG.Core;
using PACG.Data;

namespace PACG.Gameplay
{
    public class CrowbarLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        public CrowbarLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (action is not PlayCardAction playAction) return null;
            
            var modifier = new CheckModifier(action.Card);
            
            // Only restrict to Strength skill on non-Lock or Obstacle Barriers.
            if (!IsLockObstacleBarrier())
                modifier.RequiredTraits.Add("Strength");

            // If not freely, Reveal to add 1d8.
            if (!playAction.IsFreely)
                modifier.AddedDice.Add(8);

            // Recharge to add another 1d8.
            if (action.ActionType == ActionType.Recharge)
                modifier.AddedDice.Add(8);

            return modifier;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            if (!IsCardPlayable(card)) return new List<IStagedAction>();
            
            // We can reveal or reveal and recharge if not revealed already.
            if (card.CurrentLocation != CardLocation.Revealed)
            {
                return new List<IStagedAction>
                {
                    new PlayCardAction(card, ActionType.Reveal),
                    new PlayCardAction(card, ActionType.Recharge)
                };
            }
            
            // Otherwise, if we're playable that means we've revealed. We can freely recharge.
            return new List<IStagedAction> { new PlayCardAction(card, ActionType.Recharge, ("IsFreely", true)) };
        }

        private bool IsCardPlayable(CardInstance card)
        {
            if (Check == null)
                return false; // Must be in a check...

            if (_contexts.CurrentResolvable is not CheckResolvable resolvable)
                return false; // ... for a CheckResolvable...

            if (resolvable.Character != card.Owner)
                return false; // ... for the card's owner...

            if (resolvable.IsCardTypeStaged(card.CardType))
                return false; // ... with no Items played.

            if (Check.Invokes("Strength"))
                return true; // We can play on Strength checks...

            if (IsLockObstacleBarrier())
                return true; // ... or Lock or Obstacle Barriers.

            return false;
        }

        private bool IsLockObstacleBarrier()
        {
            if (_contexts.EncounterContext?.CardData.cardType != CardType.Barrier)
                return false;

            var traits = _contexts.EncounterContext.CardData.traits;
            return (traits.Contains("Lock") || traits.Contains("Obstacle"));
        }
    }
}
