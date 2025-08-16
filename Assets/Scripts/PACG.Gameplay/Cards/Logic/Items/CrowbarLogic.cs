using System.Collections.Generic;
using System.Linq;

namespace PACG.Gameplay
{
    public class CrowbarLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;

        private CheckContext Check => _contexts.CheckContext;

        private PlayCardAction RevealAction(CardInstance card) => new(card, PF.ActionType.Reveal);
        private PlayCardAction RechargeAction(CardInstance card) => new(card, PF.ActionType.Recharge);

        public CrowbarLogic(GameServices gameServices) : base(gameServices) 
        {
            _contexts = gameServices.Contexts;
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            List<IStagedAction> actions = new();
            if (IsCardPlayabe(card))
            {
                // We can reveal if not revealed already.
                if (card.CurrentLocation != CardLocation.Revealed)
                    actions.Add(RevealAction(card));

                // We can always recharge if playable.
                actions.Add(RechargeAction(card));
            }
            return actions;
        }

        private bool IsCardPlayabe(CardInstance card)
        {
            if (Check == null)
                return false; // Must be in a check...

            if (_contexts.CurrentResolvable is not SkillResolvable resolvable)
                return false; // ... for a SkillResolvable...

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

        public override void Execute(CardInstance card, IStagedAction action)
        {
            // Reveal to add 1d8.
            Check.DicePool.AddDice(1, 8);

            // Recharge to add another 1d8.
            if (action.ActionType == PF.ActionType.Recharge)
                Check.DicePool.AddDice(1, 8);
        }
    }
}
