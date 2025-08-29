using System.Collections.Generic;
using PACG.Core;

namespace PACG.Gameplay
{
    public class PrayerLogic : CardLogicBase
    {
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public PrayerLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override CheckModifier GetCheckModifier(IStagedAction action)
        {
            if (!action.ActionData.TryGetValue("Bless", out var isBless)) return null;
            if (!(bool)isBless) return null;
            
            var modifier = new CheckModifier(action.Card);
            modifier.SkillDiceToAdd++;
            return modifier;
        }

        public override void OnCommit(IStagedAction action)
        {
            if (!action.ActionData.TryGetValue("Bless", out var isBless)) return;
            if ((bool)isBless) return;
            
            // Examine the top card of your location...
            var examineResolvable = new ExamineResolvable(action.Card.Owner.Location, 1);

            // Then you may explore.
            var exploreOptionResolvable = CardEffects.CreateExploreChoice(_gameServices);
                
            examineResolvable.OverrideNextProcessor(
                new NewResolvableProcessor(exploreOptionResolvable, _gameServices)
            );
                
            _gameFlow.QueueNextProcessor(new NewResolvableProcessor(examineResolvable, _gameServices));
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            if (CanBless(card))
            {
                actions.Add(new PlayCardAction(card, ActionType.Discard, ("Bless", true)));
            }
            else if (_contexts.IsExplorePossible && _contexts.TurnContext.Character == card.Owner)
            {
                actions.Add(new PlayCardAction(card, ActionType.Discard, ("Bless", false)));
            }

            return actions;
        }

        // We can bless on any check.
        private bool CanBless(CardInstance card) =>
            _contexts.CurrentResolvable is CheckResolvable resolvable
            && !resolvable.IsCardTypeStaged(card.CardType);
    }
}
