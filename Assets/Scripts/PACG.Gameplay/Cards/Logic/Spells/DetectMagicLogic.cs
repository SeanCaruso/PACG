using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class DetectMagicLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;
        
        public DetectMagicLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override void OnCommit(IStagedAction action)
        {
            // Examine the top card of your location...
            var examineResolvable = new ExamineResolvable(action.Card.Owner.Location, 1);

            // If it's a magic card, you may encounter it.
            var topCard = action.Card.Owner.Location.ExamineTop(1)[0];
            IResolvable nextResolvable = null;
            if (topCard.Traits.Contains("Magic"))
            {
                if (action.Card.Owner == _contexts.TurnContext.Character)
                    nextResolvable = CardEffects.CreateExploreChoice(_gameServices);
            }
            else
            {
                nextResolvable = new PlayerChoiceResolvable("Shuffle?",
                    new PlayerChoiceResolvable.ChoiceOption("Shuffle", () => action.Card.Owner.Location.Shuffle()),
                    new PlayerChoiceResolvable.ChoiceOption("Skip Shuffle", () => { }));

            }

            if (nextResolvable != null)
            {
                var nextProcessor = new NewResolvableProcessor(nextResolvable, _gameServices);
                examineResolvable.OverrideNextProcessor(nextProcessor);
            }

            _gameFlow.QueueNextProcessor(new NewResolvableProcessor(examineResolvable, _gameServices));
                
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            
            if (_contexts.AreCardsPlayable && card.Owner.Location.Count > 0)
            {
                actions.Add(new PlayCardAction(card, ActionType.Banish));
            }
            
            return actions;
        }

        public override IResolvable GetRecoveryResolvable(CardInstance card)
        {
            if (!card.Owner.IsProficient(card.Data)) return null;
            
            var resolvable = new CheckResolvable(
                card,
                card.Owner,
                CardUtils.SkillCheck(5, Skill.Arcane, Skill.Divine))
            {
                OnSuccess = () => card.Owner.Reload(card),
                OnFailure = () => card.Owner.Discard(card)
            };

            return CardUtils.CreateDefaultRecoveryResolvable(resolvable, _gameServices);
        }
    }
}
