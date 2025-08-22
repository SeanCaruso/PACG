using System.Collections.Generic;
using System.Linq;

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

        public override void Execute(CardInstance card, IStagedAction action, DicePool dicePool)
        {
            if (action is not PlayCardAction playAction) return;
            if (!playAction.ActionData.TryGetValue("Bless", out var isBless)) return;

            // Discard to bless.
            if ((bool)isBless)
            {
                _contexts.CheckContext.BlessingCount++;
            }
            // This one's complex...
            else
            {
                // Examine the top card of your location...
                var examineResolvable = new ExamineResolvable(card.Owner.Location, 1);

                // Then you may explore.
                var exploreOptionResolvable = CardEffects.CreateExploreChoice(_gameServices);
                
                examineResolvable.OverrideNextProcessor(
                    new NewResolvableProcessor(exploreOptionResolvable, _gameServices)
                );
                
                _gameFlow.QueueNextProcessor(new NewResolvableProcessor(examineResolvable, _gameServices));
            }
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();
            if (CanBless(card))
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Discard, ("Bless", true)));
            }
            else if (_contexts.IsExplorePossible && _contexts.TurnContext.Character == card.Owner)
            {
                actions.Add(new PlayCardAction(card, PF.ActionType.Discard, ("Bless", false)));
            }

            return actions;
        }

        // We can bless on any check.
        private bool CanBless(CardInstance _) => 
            _contexts.CheckContext != null &&
            _contexts.CurrentResolvable is CheckResolvable &&
            !_contexts.CheckContext.StagedCardTypes.Contains(PF.CardType.Blessing);
    }
}
