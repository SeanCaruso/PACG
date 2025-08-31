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

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            if (!IsCardPlayable(card)) return new List<IStagedAction>();
            
            // Only restrict to Strength skill on non-Lock or Obstacle Barriers.
            var requiredTraits = new List<string>();
            if (!IsLockObstacleBarrier())
                requiredTraits.Add("Strength");
            
            var actions = new List<IStagedAction>();
            // We can reveal for +1d8 or reveal and recharge for +2d8 if not revealed already.
            if (card.CurrentLocation != CardLocation.Revealed)
            {
                var oneD8Modifier = new CheckModifier(card)
                {
                    AddedDice = new List<int> { 8 },
                    RequiredTraits = requiredTraits
                };
                actions.Add(new PlayCardAction(card, ActionType.Reveal, oneD8Modifier));
                var twoD8Modifier = new CheckModifier(card)
                {
                    AddedDice = new List<int> { 8, 8 },
                    RequiredTraits = requiredTraits
                };
                actions.Add(new PlayCardAction(card, ActionType.Recharge, twoD8Modifier));
            }
            // Otherwise, if we're playable that means we've revealed. We can freely recharge for +1d8.
            else
            {
                var rechargeModifier = new CheckModifier(card)
                {
                    AddedDice = new List<int>() { 8 },
                    RequiredTraits = requiredTraits
                };
                actions.Add(new PlayCardAction(card, ActionType.Recharge, rechargeModifier, ("IsFreely", true)));
            }

            return actions;
            
        }

        private bool IsCardPlayable(CardInstance card)
        {
            if (Check == null)
                return false; // Must be in a check...

            if (_contexts.CurrentResolvable is not CheckResolvable resolvable)
                return false; // ... for a CheckResolvable...

            if (resolvable.Character != card.Owner)
                return false; // ... for the card's owner...

            if (!resolvable.CanStageType(card.CardType))
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
