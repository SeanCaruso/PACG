using System.Collections.Generic;
using System.Linq;
using PACG.Core;

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
            
            // We can reveal or reveal and recharge if not revealed already.
            if (card.CurrentLocation != CardLocation.Revealed)
            {
                return new List<IStagedAction>
                {
                    new PlayCardAction(card, PF.ActionType.Reveal),
                    new PlayCardAction(card, PF.ActionType.Recharge)
                };
            }
            
            // Otherwise, if we're playable that means we've revealed. We can freely recharge.
            return new List<IStagedAction> { new PlayCardAction(card, PF.ActionType.Recharge, ("IsFreely", true)) };
        }

        private bool IsCardPlayable(CardInstance card)
        {
            if (Check == null)
                return false; // Must be in a check...

            if (_contexts.CurrentResolvable is not CheckResolvable resolvable)
                return false; // ... for a CheckResolvable...

            if (resolvable.Character != card.Owner)
                return false; // ... for the card's owner...

            if (Check.StagedCardTypes.Contains(card.Data.cardType))
                return false; // ... with no Items played.

            if (Check.CanUseSkill(PF.Skill.Strength))
                return true; // We can play on Strength checks...

            if (IsLockObstacleBarrier())
                return true; // ... or Lock or Obstacle Barriers.

            return false;
        }

        private bool IsLockObstacleBarrier()
        {
            if (_contexts.EncounterContext?.CardData.cardType != PF.CardType.Barrier)
                return false;

            var traits = _contexts.EncounterContext.CardData.traits;
            return (traits.Contains("Lock") || traits.Contains("Obstacle"));
        }

        public override void OnStage(CardInstance card, IStagedAction action)
        {
            // Only restrict to Strength skill on non-Lock or Obstacle Barriers.
            if (!IsLockObstacleBarrier())
                Check.RestrictValidSkills(card, PF.Skill.Strength);
        }

        public override void OnUndo(CardInstance card, IStagedAction action)
        {
            Check.UndoSkillModification(card);
        }

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            if (action is not PlayCardAction playAction) return;

            var isFreely = playAction.ActionData.TryGetValue("IsFreely", out var isFreelyObj) &&
                           isFreelyObj is true;

            // If not freely, Reveal to add 1d8.
            if (!isFreely)
                dicePool.AddDice(1, 8);

            // Recharge to add another 1d8.
            if (action.ActionType == PF.ActionType.Recharge)
                dicePool.AddDice(1, 8);
        }
    }
}
