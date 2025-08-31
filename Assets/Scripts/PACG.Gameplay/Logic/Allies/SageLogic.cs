using System.Collections.Generic;
using System.Linq;
using PACG.Core;

namespace PACG.Gameplay
{
    public class SageLogic : CardLogicBase
    {
        // Dependency injection
        private readonly ContextManager _contexts;
        private readonly GameFlowManager _gameFlow;
        private readonly GameServices _gameServices;

        public SageLogic(GameServices gameServices) : base(gameServices)
        {
            _contexts = gameServices.Contexts;
            _gameFlow = gameServices.GameFlow;
            _gameServices = gameServices;
        }

        public override void OnCommit(IStagedAction action)
        {
            if (action.ActionType != ActionType.Discard) return;

            // Examine the top card of your location...
            var examineResolvable = new ExamineResolvable(action.Card.Owner.Location, 1);

            // Then shuffle...
            examineResolvable.OverrideNextProcessor(
                new ShuffleDeckProcessor(action.Card.Owner.Location.Deck, _gameServices)
            );

            _gameFlow.QueueNextProcessor(new NewResolvableProcessor(examineResolvable, _gameServices));

            if (action.Card.Owner != _contexts.TurnContext.Character) return;

            // Then you may explore.
            var exploreOptionResolvable = CardEffects.CreateExploreChoice(_gameServices);
            _gameFlow.QueueNextProcessor(new NewResolvableProcessor(exploreOptionResolvable, _gameServices));
        }

        protected override List<IStagedAction> GetAvailableCardActions(CardInstance card)
        {
            var actions = new List<IStagedAction>();

            // Can recharge for +1d6 on a local Arcane or Knowledge non-combat check.
            if (_contexts.CheckContext?.Character.LocalCharacters.Contains(card.Owner) == true
                && _contexts.CurrentResolvable?.CanStageType(card.CardType) == true
                && _contexts.CheckContext.IsSkillValid
                && _contexts.CheckContext.CanUseSkill(Skill.Arcane, Skill.Knowledge))
            {
                var modifier = new CheckModifier(card)
                {
                    RestrictedCategory = CheckCategory.Skill,
                    RestrictedSkills = new[] { Skill.Arcane, Skill.Knowledge }.ToList(),
                    AddedDice = new[] { 6 }.ToList()
                };
                actions.Add(new PlayCardAction(card, ActionType.Recharge, modifier));
            }

            // Can discard to examine and shuffle.
            if (_contexts.AreCardsPlayable && card.Owner.Location.Count > 0)
            {
                actions.Add(new PlayCardAction(card, ActionType.Discard, null));
            }

            return actions;
        }
    }

    public class ShuffleDeckProcessor : BaseProcessor
    {
        private readonly Deck _deck;

        public ShuffleDeckProcessor(Deck deck, GameServices gameServices) : base(gameServices)
        {
            _deck = deck;
        }

        protected override void OnExecute()
        {
            _deck.Shuffle();
        }
    }
}
