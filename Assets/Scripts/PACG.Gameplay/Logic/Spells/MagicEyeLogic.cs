using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class MagicEyeLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public MagicEyeLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override void OnCommit(IStagedAction action)
        {
            // Examine the top 3 cards of your location.
            var examineResolvable = new ExamineResolvable(action.Card.Owner.Location, 3);

            _gameFlow.QueueNextProcessor(new NewResolvableProcessor(examineResolvable, _gameServices));
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            if (_contexts.AreCardsPlayable && card.Owner.Location.Count > 0)
            {
                actions.Add(new PlayCardAction(card, ActionType.Banish, null));
            }

            return actions;
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return null;

            var resolvable = new CheckResolvable(
                card,
                card.Owner,
                CardUtils.SkillCheck(9, Skill.Arcane, Skill.Divine))
            {
                OnSuccess = () => card.Owner.Recharge(card),
                OnFailure = () => card.Owner.Discard(card)
            };

            return CardUtils.CreateDefaultRecoveryResolvable(resolvable, _gameServices);
        }
    }
}
